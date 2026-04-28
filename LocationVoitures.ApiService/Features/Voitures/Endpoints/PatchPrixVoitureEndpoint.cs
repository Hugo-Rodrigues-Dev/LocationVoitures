using FluentValidation;
using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.Voitures.Mappings;
using LocationVoitures.ApiService.Features.Voitures.Requests;
using LocationVoitures.ApiService.Features.Voitures.Responses;
using LocationVoitures.ApiService.Common.OpenApi;
using LocationVoitures.ApiService.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures.Endpoints;

public static class PatchPrixVoitureEndpoint
{
    public static void MapPatchPrixVoiture(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/voitures/{id:int}/prix", async (int id, UpdatePrixVoitureRequest request, IValidator<UpdatePrixVoitureRequest> validator, RentalDbContext db) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return validation.ToValidationProblem();
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
        .WithName("PatchPrixVoiture")
        .WithTags(OpenApiDescriptions.VoituresTag)
        .WithSummary("Modifie le prix journalier d'une voiture")
        .WithDescription("Met a jour uniquement le prix de location par jour d'une voiture.")
        .Produces<VoitureDto>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
