using LocationVoitures.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures;

public static class PatchPrixVoitureEndpoint
{
    public static void MapPatchPrixVoiture(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/voitures/{id:int}/prix", async (int id, UpdatePrixVoitureRequest request, RentalDbContext db) =>
        {
            if (request.PrixLocationParJour <= 0)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["PrixLocationParJour"] = ["Le prix doit etre strictement positif."]
                });
            }

            var voiture = await db.Voitures.FirstOrDefaultAsync(v => v.Id == id);
            if (voiture is null)
            {
                return Results.NotFound();
            }

            voiture.PrixLocationParJour = request.PrixLocationParJour;
            await db.SaveChangesAsync();

            return Results.Ok(voiture.ToDto(true));
        })
        .WithName("PatchPrixVoiture");
    }
}
