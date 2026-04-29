var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(pgAdmin =>
    {
        pgAdmin.WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@locationvoitures.fr");
        pgAdmin.WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "Admin123!");
    });

var rentalDb = postgres.AddDatabase("RentalDb");

var paymentService = builder.AddProject<Projects.LocationVoitures_PaymentService>("paymentservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

var apiService = builder.AddProject<Projects.LocationVoitures_ApiService>("apiservice")
    .WithReference(rentalDb)
    .WaitFor(rentalDb)
    .WithReference(paymentService)
    .WaitFor(paymentService)
    .WithHttpHealthCheck("/health");

var gateway = builder.AddProject<Projects.LocationVoitures_Gateway>("gateway")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.LocationVoitures_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(gateway)
    .WaitFor(gateway);

builder.Build().Run();
