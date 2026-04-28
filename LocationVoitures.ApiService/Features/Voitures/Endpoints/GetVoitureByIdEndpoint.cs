using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.Voitures.Responses;
using LocationVoitures.ApiService.Common.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures.Endpoints;

public static class GetVoitureByIdEndpoint
{
    public static void MapGetVoitureById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/voitures/{id:int}", async (int id, RentalDbContext db) =>
        {
            var dateReference = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var voiture = await db.Voitures
                .Where(v => v.Id == id)
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
        .WithName("GetVoitureById")
        .WithTags(OpenApiDescriptions.VoituresTag)
        .WithSummary("Recupere une voiture par identifiant")
        .WithDescription("Retourne les informations d'une voiture a partir de son identifiant technique.")
        .Produces<VoitureDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
