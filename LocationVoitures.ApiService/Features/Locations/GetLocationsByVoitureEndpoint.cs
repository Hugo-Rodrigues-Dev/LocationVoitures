using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.OpenApi;
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
                .Select(location => new LocationDto
                {
                    Id = location.Id,
                    Immatriculation = location.Voiture!.Immatriculation,
                    LoueurId = location.LoueurId,
                    DateDebut = location.DateDebut,
                    DateFin = location.DateFin,
                    Annule = location.Annule
                })
                .ToListAsync();

            return Results.Ok(locations);
        })
        .WithName("GetLocationsByVoiture")
        .WithTags(OpenApiDescriptions.LocationsTag)
        .WithSummary("Liste les locations d'une voiture")
        .WithDescription("Retourne toutes les reservations associees a une voiture, recherchee par immatriculation.")
        .Produces<List<LocationDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
