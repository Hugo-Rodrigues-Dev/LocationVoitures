using FluentValidation;
using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Domain;
using LocationVoitures.ApiService.Features.OpenApi;
using LocationVoitures.ApiService.Features.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures;

public static class CreateVoitureEndpoint
{
    public static void MapCreateVoiture(this IEndpointRouteBuilder app)
    {
        app.MapPost("/voitures", async (CreateVoitureRequest request, IValidator<CreateVoitureRequest> validator, RentalDbContext db) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return validation.ToValidationProblem();
            }

            var immatriculationExiste = await db.Voitures
                .AnyAsync(voiture => voiture.Immatriculation == request.Immatriculation);

            if (immatriculationExiste)
            {
                return Results.Conflict($"Une voiture avec l'immatriculation {request.Immatriculation} existe deja.");
            }

            var voiture = new Voiture
            {
                Immatriculation = request.Immatriculation,
                Marque = request.Marque,
                Modele = request.Modele,
                Categorie = request.Categorie,
                PrixLocationParJour = request.PrixLocationParJour
            };

            db.Voitures.Add(voiture);
            await db.SaveChangesAsync();

            return Results.Created($"/voitures/{voiture.Id}", voiture.ToDto(true));
        })
        .WithName("CreateVoiture")
        .WithTags(OpenApiDescriptions.VoituresTag)
        .WithSummary("Cree une voiture")
        .WithDescription("Ajoute une nouvelle voiture dans le catalogue. L'immatriculation doit etre unique.")
        .Produces<VoitureDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .Produces<string>(StatusCodes.Status409Conflict, "text/plain")
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
