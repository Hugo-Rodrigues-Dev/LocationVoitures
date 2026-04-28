using FluentValidation;
using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.Voitures.Mappings;
using LocationVoitures.ApiService.Features.Voitures.Requests;
using LocationVoitures.ApiService.Features.Voitures.Responses;
using LocationVoitures.ApiService.Common.OpenApi;
using LocationVoitures.ApiService.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures.Endpoints;

public static class UpdateVoitureEndpoint
{
    public static void MapUpdateVoiture(this IEndpointRouteBuilder app)
    {
        app.MapPut("/voitures/{id:int}", async (int id, CreateVoitureRequest request, IValidator<CreateVoitureRequest> validator, RentalDbContext db) =>
        {
            var voiture = await db.Voitures.FirstOrDefaultAsync(v => v.Id == id);
            if (voiture is null)
            {
                return Results.NotFound();
            }

            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return validation.ToValidationProblem();
            }

            var immatriculationExiste = await db.Voitures
                .AnyAsync(v => v.Id != id && v.Immatriculation == request.Immatriculation);

            if (immatriculationExiste)
            {
                return Results.Conflict($"Une voiture avec l'immatriculation {request.Immatriculation} existe deja.");
            }

            voiture.Immatriculation = request.Immatriculation;
            voiture.Marque = request.Marque;
            voiture.Modele = request.Modele;
            voiture.Categorie = request.Categorie;
            voiture.PrixLocationParJour = request.PrixLocationParJour;

            await db.SaveChangesAsync();

            return Results.Ok(voiture.ToDto(true));
        })
        .WithName("UpdateVoiture")
        .WithTags(OpenApiDescriptions.VoituresTag)
        .WithSummary("Remplace une voiture")
        .WithDescription("Met a jour l'ensemble des informations d'une voiture a partir de son identifiant.")
        .Produces<VoitureDto>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .Produces<string>(StatusCodes.Status409Conflict, "text/plain")
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
