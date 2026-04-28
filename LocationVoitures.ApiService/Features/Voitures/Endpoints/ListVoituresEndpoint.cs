using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.Voitures.Responses;
using LocationVoitures.ApiService.Common.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Features.Voitures.Endpoints;

public static class ListVoituresEndpoint
{
    public static void MapListVoitures(this IEndpointRouteBuilder app)
    {
        app.MapGet("/voitures", async (RentalDbContext db, string? categorie) =>
        {
            var dateReference = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var query = db.Voitures.AsQueryable();

            if (!string.IsNullOrWhiteSpace(categorie))
            {
                query = query.Where(voiture => voiture.Categorie == categorie);
            }

            var voitures = await query
                .OrderBy(voiture => voiture.Marque)
                .ThenBy(voiture => voiture.Modele)
                .Select(voiture => new VoitureDto
                {
                    Immatriculation = voiture.Immatriculation,
                    Marque = voiture.Marque,
                    Modele = voiture.Modele,
                    Categorie = voiture.Categorie ?? string.Empty,
                    PrixParJour = voiture.PrixLocationParJour,
                    EstDisponible = !db.Locations.Any(location =>
                        location.VoitureId == voiture.Id &&
                        !location.Annule &&
                        location.DateDebut <= dateReference &&
                        location.DateFin >= dateReference)
                })
                .ToListAsync();

            return Results.Ok(voitures);
        })
        .WithName("GetVoitures")
        .WithTags(OpenApiDescriptions.VoituresTag)
        .WithSummary("Liste les voitures")
        .WithDescription("Retourne le catalogue des voitures disponibles dans l'application. Le filtre de categorie est optionnel et la disponibilite est calculee a la date du jour.")
        .Produces<List<VoitureDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
