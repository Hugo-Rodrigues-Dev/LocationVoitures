using LocationVoitures.ApiService.Domain;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Data;

public class RentalDbContext(DbContextOptions<RentalDbContext> options) : DbContext(options)
{
    public DbSet<Loueur> Loueurs => Set<Loueur>();
    public DbSet<Voiture> Voitures => Set<Voiture>();
    public DbSet<Location> Locations => Set<Location>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Loueur>()
            .Property(loueur => loueur.Pays)
            .HasDefaultValue("France");
    }
}
