using FluentValidation;
using LocationVoitures.PaymentService.Features.Payments.Endpoints;
using LocationVoitures.PaymentService.Features.Payments.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<CardValidationService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new
{
    Application = "Location Voitures Payment Service",
    Version = "v1",
    Endpoints = new[] { "/payments/authorize" }
}))
.WithTags("Informations")
.WithSummary("Donne des informations de base sur le service de paiement")
.Produces(StatusCodes.Status200OK);

app.MapAuthorizePayment();
app.MapDefaultEndpoints();

app.Run();
