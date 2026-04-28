using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Data;

public static class DbInitializerExtensions
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RentalDbContext>();

        await dbContext.Database.MigrateAsync();
        await SeedData.InitializeAsync(dbContext);
    }
}
