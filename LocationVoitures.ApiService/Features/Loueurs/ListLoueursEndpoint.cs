using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Loueurs;

public static class ListLoueursEndpoint
{
    public static void MapListLoueurs(this IEndpointRouteBuilder app)
    {
        app.MapGet("/loueurs", async (RentalDbContext db) =>
        {
            var loueurs = await db.Loueurs
                .OrderBy(loueur => loueur.Nom)
                .ThenBy(loueur => loueur.Prenom)
                .Select(loueur => new LoueurDto
                {
                    Id = loueur.Id,
                    Nom = loueur.Nom,
                    Prenom = loueur.Prenom,
                    Mobile = loueur.Mobile,
                    Email = loueur.Email,
                    Pays = loueur.Pays,
                    EstBlacklist = loueur.EstBlacklist
                })
                .ToListAsync();

            return Results.Ok(loueurs);
        })
        .WithName("GetLoueurs")
        .WithTags(OpenApiDescriptions.LoueursTag)
        .WithSummary("Liste les loueurs")
        .WithDescription("Retourne la liste des loueurs connus par l'application avec leur statut de blacklist.")
        .Produces<List<LoueurDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
