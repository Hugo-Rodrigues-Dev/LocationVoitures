using LocationVoitures.ApiService.Domain;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Data;

public static class SeedData
{
    public static async Task InitializeAsync(RentalDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Voitures.AnyAsync(cancellationToken))
        {
            return;
        }

        var loueurs = new[]
        {
            new Loueur
            {
                Nom = "Martin",
                Prenom = "Claire",
                Mobile = "0601020304",
                Email = "claire.martin@example.com",
                Pays = "France"
            },
            new Loueur
            {
                Nom = "Bernard",
                Prenom = "Lucas",
                Mobile = "0605060708",
                Email = "lucas.bernard@example.com",
                Pays = "France"
            }
        };

        var voitures = new[]
        {
            new Voiture
            {
                Immatriculation = "AA-123-BB",
                Marque = "Renault",
                Modele = "Clio",
                Categorie = "Citadine",
                PrixLocationParJour = 45.00m
            },
            new Voiture
            {
                Immatriculation = "CC-456-DD",
                Marque = "Peugeot",
                Modele = "308",
                Categorie = "Berline",
                PrixLocationParJour = 62.50m
            },
            new Voiture
            {
                Immatriculation = "EE-789-FF",
                Marque = "Dacia",
                Modele = "Duster",
                Categorie = "SUV",
                PrixLocationParJour = 80.00m
            }
        };

        await dbContext.Loueurs.AddRangeAsync(loueurs, cancellationToken);
        await dbContext.Voitures.AddRangeAsync(voitures, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var locations = new[]
        {
            new Location
            {
                VoitureId = voitures[1].Id,
                LoueurId = loueurs[0].Id,
                DateDebut = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                DateFin = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(2)),
                Annule = false,
                Paye = true
            },
            new Location
            {
                VoitureId = voitures[2].Id,
                LoueurId = loueurs[1].Id,
                DateDebut = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(5)),
                DateFin = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
                Annule = false,
                Paye = true
            }
        };

        await dbContext.Locations.AddRangeAsync(locations, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
