using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures;

public static class GetVoitureByImmatriculationEndpoint
{
    public static void MapGetVoitureByImmatriculation(this IEndpointRouteBuilder app)
    {
        app.MapGet("/voitures/immatriculation/{immatriculation}", async (string immatriculation, RentalDbContext db) =>
        {
            var dateReference = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var voiture = await db.Voitures
                .Where(v => v.Immatriculation == immatriculation)
                .Select(voiture => new VoitureDto
                {
                    Immatriculation = voiture.Immatriculation,
                    Marque = voiture.Marque,
                    Modele = voiture.Modele,
                    Categorie = voiture.Categorie ?? string.Empty,
                    PrixParJour = voiture.PrixLocationParJour,
                    EstDisponible = !db.Locations.Any(location =>
                        location.VoitureId == voiture.Id &&
                        !location.Annule &&
                        location.DateDebut <= dateReference &&
                        location.DateFin >= dateReference)
                })
                .FirstOrDefaultAsync();

            return voiture is null ? Results.NotFound() : Results.Ok(voiture);
        })
        .WithName("GetVoitureByImmatriculation")
        .WithTags(OpenApiDescriptions.VoituresTag)
        .WithSummary("Recupere une voiture par immatriculation")
        .WithDescription("Recherche une voiture par son immatriculation metier, au format AA-123-BB.")
        .Produces<VoitureDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
