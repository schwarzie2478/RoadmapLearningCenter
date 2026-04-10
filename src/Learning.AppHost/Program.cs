var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL database
var db = builder
    .AddPostgres("postgres")
    .WithDataVolume("learning-db-data")
    .AddDatabase("learning");

// Backend API
var server = builder
    .AddProject<Projects.Learning_Server>("server")
    .WithReference(db)
    .WaitFor(db);

// Blazor WASM frontend
builder.AddProject<Projects.Learning_Client>("client")
    .WithReference(server)
    .WaitFor(server);

builder.Build().Run();
