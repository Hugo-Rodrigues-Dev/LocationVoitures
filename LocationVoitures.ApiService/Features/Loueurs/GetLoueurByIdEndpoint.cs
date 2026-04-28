using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Loueurs;

public static class GetLoueurByIdEndpoint
{
    public static void MapGetLoueurById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/loueurs/{id:int}", async (int id, RentalDbContext db) =>
        {
            var loueur = await db.Loueurs
                .Where(current => current.Id == id)
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
        .WithName("GetLoueurById")
        .WithTags(OpenApiDescriptions.LoueursTag)
        .WithSummary("Recupere un loueur par identifiant")
        .WithDescription("Retourne les informations detaillees d'un loueur a partir de son identifiant technique.")
        .Produces<LoueurDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
