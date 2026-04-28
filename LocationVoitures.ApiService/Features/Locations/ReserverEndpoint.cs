using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Domain;
using LocationVoitures.ApiService.Services;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Locations;

public static class ReserverEndpoint
{
    public static void MapReserver(this IEndpointRouteBuilder app)
    {
        app.MapPost("/locations", async (ReserverRequest request, RentalDbContext db, LocationService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.Immatriculation))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["Immatriculation"] = ["L'immatriculation est obligatoire."]
                });
            }

            if (request.LoueurId <= 0)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["LoueurId"] = ["Le LoueurId doit etre strictement positif."]
                });
            }

            try
            {
                service.ValiderPeriode(request.DateDebut, request.DateFin);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["Dates"] = [exception.Message]
                });
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

            var nouvelleLocation = new Location
            {
                VoitureId = voiture.Id,
                LoueurId = loueur.Id,
                DateDebut = request.DateDebut,
                DateFin = request.DateFin,
                Annule = false
            };

            db.Locations.Add(nouvelleLocation);
            await db.SaveChangesAsync();

            return Results.Ok(new ReserverResponse(nouvelleLocation.Id, prix, "Reservation confirmee"));
        })
        .WithName("CreerLocation");
    }
}
