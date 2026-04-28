var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(pgAdmin =>
    {
        pgAdmin.WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@locationvoitures.local");
        pgAdmin.WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "Admin123!");
    });

var rentalDb = postgres.AddDatabase("RentalDb");

var apiService = builder.AddProject<Projects.LocationVoitures_ApiService>("apiservice")
    .WithReference(rentalDb)
    .WaitFor(rentalDb)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.LocationVoitures_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
