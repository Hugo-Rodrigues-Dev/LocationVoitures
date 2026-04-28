using FluentValidation;
using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Domain;
using LocationVoitures.ApiService.Features.Loueurs.Mappings;
using LocationVoitures.ApiService.Features.Loueurs.Requests;
using LocationVoitures.ApiService.Features.Loueurs.Responses;
using LocationVoitures.ApiService.Common.OpenApi;
using LocationVoitures.ApiService.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Loueurs.Endpoints;

public static class CreateLoueurEndpoint
{
    public static void MapCreateLoueur(this IEndpointRouteBuilder app)
    {
        app.MapPost("/loueurs", async (CreateLoueurRequest request, IValidator<CreateLoueurRequest> validator, RentalDbContext db) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return validation.ToValidationProblem();
            }

            var mobileExiste = await db.Loueurs.AnyAsync(loueur => loueur.Mobile == request.Mobile);
            if (mobileExiste)
            {
                return Results.Conflict($"Un loueur avec le mobile {request.Mobile} existe deja.");
            }

            var loueur = new Loueur
            {
                Nom = request.Nom,
                Prenom = request.Prenom,
                Mobile = request.Mobile,
                Email = request.Email,
                Pays = request.Pays,
                EstBlacklist = false
            };

            db.Loueurs.Add(loueur);
            await db.SaveChangesAsync();

            return Results.Created($"/loueurs/{loueur.Id}", loueur.ToDto());
        })
        .WithName("CreateLoueur")
        .WithTags(OpenApiDescriptions.LoueursTag)
        .WithSummary("Cree un loueur")
        .WithDescription("Ajoute un nouveau loueur dans l'application. Le numero de mobile doit etre unique.")
        .Produces<LoueurDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status409Conflict, "text/plain")
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
