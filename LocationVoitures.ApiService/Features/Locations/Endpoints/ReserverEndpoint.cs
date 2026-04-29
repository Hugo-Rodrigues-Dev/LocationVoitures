using FluentValidation;
using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Domain;
using LocationVoitures.ApiService.Features.Locations.Requests;
using LocationVoitures.ApiService.Features.Locations.Responses;
using LocationVoitures.ApiService.Common.OpenApi;
using LocationVoitures.ApiService.Common.Validation;
using LocationVoitures.ApiService.Features.Paiements.Requests;
using LocationVoitures.ApiService.Features.Paiements.Services;
using LocationVoitures.ApiService.Services;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Locations.Endpoints;

public static class ReserverEndpoint
{
    public static void MapReserver(this IEndpointRouteBuilder app)
    {
        app.MapPost("/locations", async (ReserverRequest request, IValidator<ReserverRequest> validator, RentalDbContext db, LocationService service, PaiementApiClient paiementApiClient) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return validation.ToValidationProblem();
            }

            var voiture = await db.Voitures.FirstOrDefaultAsync(v => v.Immatriculation == request.Immatriculation);
            if (voiture is null)
            {
                return Results.NotFound($"Aucune voiture trouvee pour l'immatriculation {request.Immatriculation}.");
            }

            var loueur = await db.Loueurs.FirstOrDefaultAsync(l => l.Id == request.LoueurId);
            if (loueur is null)
            {
                return Results.NotFound($"Aucun loueur trouve pour l'identifiant {request.LoueurId}.");
            }

            try
            {
                service.ValiderLoueurAutorise(loueur.EstBlacklist);
            }
            catch (InvalidOperationException exception)
            {
                return Results.Conflict(exception.Message);
            }

            var indisponible = await db.Locations.AnyAsync(location =>
                location.VoitureId == voiture.Id &&
                !location.Annule &&
                request.DateDebut <= location.DateFin &&
                request.DateFin >= location.DateDebut);

            if (indisponible)
            {
                return Results.Conflict("La voiture n'est pas disponible sur cette periode.");
            }

            var prix = service.CalculerPrixTotal(voiture.PrixLocationParJour, request.DateDebut, request.DateFin);

            try
            {
                var paiement = await paiementApiClient.AutoriserAsync(new AutoriserPaiementRequest
                {
                    CardNumber = request.NumeroCarte,
                    ExpirationMonth = request.MoisExpiration,
                    ExpirationYear = request.AnneeExpiration,
                    Amount = prix,
                    Currency = "EUR"
                });

                if (!paiement.IsAuthorized)
                {
                    return Results.Problem(
                        title: "Paiement refuse",
                        detail: paiement.Message,
                        statusCode: StatusCodes.Status402PaymentRequired);
                }
            }
            catch (HttpRequestException)
            {
                return Results.Problem(
                    title: "Service de paiement indisponible",
                    detail: "Le service de paiement ne repond pas pour le moment. Reessayez dans quelques instants.",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (InvalidOperationException)
            {
                return Results.Problem(
                    title: "Paiement indisponible",
                    detail: "La reponse du service de paiement est invalide ou incomplete.",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            var nouvelleLocation = new Location
            {
                VoitureId = voiture.Id,
                LoueurId = loueur.Id,
                DateDebut = request.DateDebut,
                DateFin = request.DateFin,
                Annule = false,
                Paye = true
            };

            db.Locations.Add(nouvelleLocation);
            await db.SaveChangesAsync();

            return Results.Ok(new ReserverResponse
            {
                IdLocation = nouvelleLocation.Id,
                PrixTotal = prix,
                Message = "Reservation confirmee et paiement accepte",
                PaiementConfirme = true
            });
        })
        .WithName("CreerLocation")
        .WithTags(OpenApiDescriptions.LocationsTag)
        .WithSummary("Cree une reservation")
        .WithDescription("Verifie les regles metier puis cree une location si la voiture et le loueur existent et si la periode est disponible.")
        .Produces<ReserverResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status402PaymentRequired)
        .Produces<string>(StatusCodes.Status404NotFound, "text/plain")
        .Produces<string>(StatusCodes.Status409Conflict, "text/plain")
        .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
