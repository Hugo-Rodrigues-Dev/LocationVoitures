using FluentValidation;
using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Domain;
using LocationVoitures.ApiService.Features.Voitures.Mappings;
using LocationVoitures.ApiService.Features.Voitures.Requests;
using LocationVoitures.ApiService.Features.Voitures.Responses;
using LocationVoitures.ApiService.Features.Voitures.Validators;
using LocationVoitures.ApiService.Common.OpenApi;
using LocationVoitures.ApiService.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures.Endpoints;

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
