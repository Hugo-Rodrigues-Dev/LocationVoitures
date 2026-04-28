using LocationVoitures.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Locations;

public static class ListLocationsEndpoint
{
    public static void MapListLocations(this IEndpointRouteBuilder app)
    {
        app.MapGet("/locations", async (RentalDbContext db) =>
        {
            var locations = await db.Locations
                .Include(location => location.Voiture)
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
        .WithName("GetLocations");
    }
}
