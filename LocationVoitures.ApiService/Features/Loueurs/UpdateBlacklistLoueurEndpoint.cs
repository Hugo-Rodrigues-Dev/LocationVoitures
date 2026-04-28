using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Loueurs;

public static class UpdateBlacklistLoueurEndpoint
{
    public static void MapUpdateBlacklistLoueur(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/loueurs/{id:int}/blacklist", async (int id, UpdateBlacklistLoueurRequest request, RentalDbContext db) =>
        {
            var loueur = await db.Loueurs.FirstOrDefaultAsync(current => current.Id == id);
            if (loueur is null)
            {
                return Results.NotFound();
            }

            loueur.EstBlacklist = request.EstBlacklist;
            await db.SaveChangesAsync();

            return Results.Ok(loueur.ToDto());
        })
        .WithName("UpdateBlacklistLoueur")
        .WithTags(OpenApiDescriptions.LoueursTag)
        .WithSummary("Modifie le statut de blacklist d'un loueur")
        .WithDescription("Permet de blacklister ou rehabiliter un loueur. Un loueur blackliste ne peut plus reserver.")
        .Produces<LoueurDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
