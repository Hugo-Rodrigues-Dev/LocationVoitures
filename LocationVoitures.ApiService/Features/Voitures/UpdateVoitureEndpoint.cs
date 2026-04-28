using LocationVoitures.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures;

public static class UpdateVoitureEndpoint
{
    public static void MapUpdateVoiture(this IEndpointRouteBuilder app)
    {
        app.MapPut("/voitures/{id:int}", async (int id, CreateVoitureRequest request, RentalDbContext db) =>
        {
            var voiture = await db.Voitures.FirstOrDefaultAsync(v => v.Id == id);
            if (voiture is null)
            {
                return Results.NotFound();
            }

            var validation = CreateVoitureEndpoint.ValidateVoitureRequest(request);
            if (validation is not null)
            {
                return validation;
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
        .WithName("UpdateVoiture");
    }
}
