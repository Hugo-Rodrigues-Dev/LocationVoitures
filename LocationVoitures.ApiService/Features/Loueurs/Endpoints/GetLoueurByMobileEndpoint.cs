using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.Loueurs.Responses;
using LocationVoitures.ApiService.Common.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Loueurs.Endpoints;

public static class GetLoueurByMobileEndpoint
{
    public static void MapGetLoueurByMobile(this IEndpointRouteBuilder app)
    {
        app.MapGet("/loueurs/mobile/{mobile}", async (string mobile, RentalDbContext db) =>
        {
            var loueur = await db.Loueurs
                .Where(current => current.Mobile == mobile)
                .Select(current => new LoueurDto
                {
                    Id = current.Id,
                    Nom = current.Nom,
                    Prenom = current.Prenom,
                    Mobile = current.Mobile,
                    Email = current.Email,
                    Pays = current.Pays,
                    EstBlacklist = current.EstBlacklist
                })
                .FirstOrDefaultAsync();

            return loueur is null ? Results.NotFound() : Results.Ok(loueur);
        })
        .WithName("GetLoueurByMobile")
        .WithTags(OpenApiDescriptions.LoueursTag)
        .WithSummary("Recupere un loueur par mobile")
        .WithDescription("Retourne un loueur a partir de son numero de mobile unique.")
        .Produces<LoueurDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
