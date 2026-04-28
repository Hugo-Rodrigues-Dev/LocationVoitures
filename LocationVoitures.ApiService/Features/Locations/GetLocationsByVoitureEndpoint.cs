using LocationVoitures.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Locations;

public static class GetLocationsByVoitureEndpoint
{
    public static void MapGetLocationsByVoiture(this IEndpointRouteBuilder app)
    {
        app.MapGet("/locations/voiture/{immatriculation}", async (string immatriculation, RentalDbContext db) =>
        {
            var locations = await db.Locations
                .Include(location => location.Voiture)
                .Where(location => location.Voiture!.Immatriculation == immatriculation)
                .OrderBy(location => location.DateDebut)
                .Select(location => new LocationDto(
                    location.Id,
                    location.Voiture!.Immatriculation,
                    location.LoueurId,
                    location.DateDebut,
                    location.DateFin,
                    location.Annule))
                .ToListAsync();

            return Results.Ok(locations);
        })
        .WithName("GetLocationsByVoiture");
    }
}
