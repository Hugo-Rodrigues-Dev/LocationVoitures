using FluentValidation;
using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures;

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
        .WithName("PatchPrixVoiture");
    }
}
