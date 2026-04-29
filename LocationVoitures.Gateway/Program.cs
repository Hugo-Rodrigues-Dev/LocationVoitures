var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire integrations (health checks, telemetry, service discovery).
builder.AddServiceDefaults();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    Application = "LocationVoitures Gateway",
    Description = "Reverse proxy YARP expose devant l'API de location.",
    RoutedPrefixes = new[]
    {
        "/voitures",
        "/locations",
        "/loueurs"
    }
}))
.WithName("GatewayRoot")
.WithSummary("Donne des informations de base sur la gateway")
.WithDescription("Endpoint d'accueil pour verifier que la gateway YARP est joignable et connaitre les prefixes routes.")
.Produces(StatusCodes.Status200OK);

app.MapReverseProxy();
app.MapDefaultEndpoints();

app.Run();
