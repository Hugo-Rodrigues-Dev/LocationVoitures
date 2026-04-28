using LocationVoitures.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures;

public static class ListVoituresEndpoint
{
    public static void MapListVoitures(this IEndpointRouteBuilder app)
    {
        app.MapGet("/voitures", async (RentalDbContext db, string? categorie) =>
        {
            var dateReference = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var query = db.Voitures.AsQueryable();

            if (!string.IsNullOrWhiteSpace(categorie))
            {
                query = query.Where(voiture => voiture.Categorie == categorie);
            }

            var voitures = await query
                .OrderBy(voiture => voiture.Marque)
                .ThenBy(voiture => voiture.Modele)
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
                .ToListAsync();

            return Results.Ok(voitures);
        })
        .WithName("GetVoitures");
    }
}
