using LocationVoitures.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures;

public static class GetVoitureByIdEndpoint
{
    public static void MapGetVoitureById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/voitures/{id:int}", async (int id, RentalDbContext db) =>
        {
            var dateReference = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var voiture = await db.Voitures
                .Where(v => v.Id == id)
                .Select(voiture => new VoitureDto(
                    voiture.Immatriculation,
                    voiture.Marque,
                    voiture.Modele,
                    voiture.Categorie ?? string.Empty,
                    voiture.PrixLocationParJour,
                    !db.Locations.Any(location =>
                        location.VoitureId == voiture.Id &&
                        !location.Annule &&
                        location.DateDebut <= dateReference &&
                        location.DateFin >= dateReference)))
                .FirstOrDefaultAsync();

            return voiture is null ? Results.NotFound() : Results.Ok(voiture);
        })
        .WithName("GetVoitureById");
    }
}
