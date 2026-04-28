using FluentValidation;
using LocationVoitures.ApiService.Data;
using LocationVoitures.ApiService.Features.Locations;
using LocationVoitures.ApiService.Features.OpenApi;
using LocationVoitures.ApiService.Features.Voitures;
using LocationVoitures.ApiService.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.AddNpgsqlDbContext<RentalDbContext>("RentalDb");
builder.Services.AddScoped<LocationService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Location Voitures API")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(
                    ScalarTarget.CSharp,
                    ScalarClient.HttpClient);
    });
}

await app.MigrateDatabaseAsync();

app.MapGet("/", () => Results.Ok(new
{
    Application = "Location Voitures API",
    Version = "v1",
    Documentation = "/scalar/v1",
    OpenApi = "/openapi/v1.json"
}))
.WithTags("Informations")
.WithSummary("Donne des informations de base sur l'API")
.WithDescription("Endpoint d'accueil permettant de verifier que l'API tourne et de retrouver rapidement les liens utiles vers la documentation.")
.Produces(StatusCodes.Status200OK);

app.MapListVoitures();
app.MapGetVoitureById();
app.MapGetVoitureByImmatriculation();
app.MapCreateVoiture();
app.MapUpdateVoiture();
app.MapPatchPrixVoiture();
app.MapReserver();
app.MapListLocations();
app.MapGetLocationsByVoiture();
app.MapAnnulerLocation();

app.MapDefaultEndpoints();

app.Run();
