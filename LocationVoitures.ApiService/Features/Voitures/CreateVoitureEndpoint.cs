using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Domain;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures;

public static class CreateVoitureEndpoint
{
    public static void MapCreateVoiture(this IEndpointRouteBuilder app)
    {
        app.MapPost("/voitures", async (CreateVoitureRequest request, RentalDbContext db) =>
        {
            var validation = ValidateVoitureRequest(request);
            if (validation is not null)
            {
                return validation;
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
        .WithName("CreateVoiture");
    }

    internal static IResult? ValidateVoitureRequest(CreateVoitureRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Immatriculation))
        {
            errors["Immatriculation"] = ["L'immatriculation est obligatoire."];
        }

        if (string.IsNullOrWhiteSpace(request.Marque))
        {
            errors["Marque"] = ["La marque est obligatoire."];
        }

        if (string.IsNullOrWhiteSpace(request.Modele))
        {
            errors["Modele"] = ["Le modele est obligatoire."];
        }

        if (request.PrixLocationParJour <= 0)
        {
            errors["PrixLocationParJour"] = ["Le prix doit etre strictement positif."];
        }

        return errors.Count > 0 ? Results.ValidationProblem(errors) : null;
    }
}
