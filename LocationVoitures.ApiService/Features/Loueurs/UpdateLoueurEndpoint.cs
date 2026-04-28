using FluentValidation;
using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.OpenApi;
using LocationVoitures.ApiService.Features.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Loueurs;

public static class UpdateLoueurEndpoint
{
    public static void MapUpdateLoueur(this IEndpointRouteBuilder app)
    {
        app.MapPut("/loueurs/{id:int}", async (int id, CreateLoueurRequest request, IValidator<CreateLoueurRequest> validator, RentalDbContext db) =>
        {
            var loueur = await db.Loueurs.FirstOrDefaultAsync(current => current.Id == id);
            if (loueur is null)
            {
                return Results.NotFound();
            }

            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return validation.ToValidationProblem();
            }

            var mobileExiste = await db.Loueurs.AnyAsync(current => current.Id != id && current.Mobile == request.Mobile);
            if (mobileExiste)
            {
                return Results.Conflict($"Un loueur avec le mobile {request.Mobile} existe deja.");
            }

            loueur.Nom = request.Nom;
            loueur.Prenom = request.Prenom;
            loueur.Mobile = request.Mobile;
            loueur.Email = request.Email;
            loueur.Pays = request.Pays;

            await db.SaveChangesAsync();

            return Results.Ok(loueur.ToDto());
        })
        .WithName("UpdateLoueur")
        .WithTags(OpenApiDescriptions.LoueursTag)
        .WithSummary("Remplace un loueur")
        .WithDescription("Met a jour toutes les informations d'un loueur existant, hors statut de blacklist.")
        .Produces<LoueurDto>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status409Conflict, "text/plain")
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
