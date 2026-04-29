using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.Locations.Responses;
using LocationVoitures.ApiService.Common.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Locations.Endpoints;

public static class ListLocationsEndpoint
{
    public static void MapListLocations(this IEndpointRouteBuilder app)
    {
        app.MapGet("/locations", async (RentalDbContext db) =>
        {
            var locations = await db.Locations
                .Include(location => location.Voiture)
                .OrderBy(location => location.DateDebut)
                .Select(location => new LocationDto
                {
                    Id = location.Id,
                    Immatriculation = location.Voiture!.Immatriculation,
                    LoueurId = location.LoueurId,
                    DateDebut = location.DateDebut,
                    DateFin = location.DateFin,
                    Annule = location.Annule,
                    Paye = location.Paye
                })
                .ToListAsync();

            return Results.Ok(locations);
        })
        .WithName("GetLocations")
        .WithTags(OpenApiDescriptions.LocationsTag)
        .WithSummary("Liste les locations")
        .WithDescription("Retourne l'historique des locations connues par l'application.")
        .Produces<List<LocationDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
