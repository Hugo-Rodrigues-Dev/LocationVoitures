using LocationVoitures.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Locations;

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

            return Results.Ok(new { location.Id, location.Annule });
        })
        .WithName("AnnulerLocation");
    }
}
