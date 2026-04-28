using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.Locations.Responses;
using LocationVoitures.ApiService.Common.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Locations.Endpoints;

public static class AnnulerLocationEndpoint
{
    public static void MapAnnulerLocation(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/locations/{id:int}/annulation", async (int id, RentalDbContext db) =>
        {
            var location = await db.Locations.FirstOrDefaultAsync(l => l.Id == id);
            if (location is null)
            {
                return Results.NotFound();
            }

            location.Annule = true;
            await db.SaveChangesAsync();

            return Results.Ok(new AnnulationLocationResponse
            {
                Id = location.Id,
                Annule = location.Annule
            });
        })
        .WithName("AnnulerLocation")
        .WithTags(OpenApiDescriptions.LocationsTag)
        .WithSummary("Annule une location")
        .WithDescription("Marque une location comme annulee a partir de son identifiant.")
        .Produces<AnnulationLocationResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
