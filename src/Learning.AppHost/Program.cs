var builder = DistributedApplication.CreateBuilder(args);

var db = builder
    .AddPostgres("postgres")
    .WithDataVolume("learning-db-data")
    .AddDatabase("learning");

var openRouterKey = builder.AddParameter("openrouter-api-key", secret: true);
var openAiKey = builder.AddParameter("openai-api-key", secret: true);

var server = builder
    .AddProject<Projects.Learning_Server>("server")
    .WithReference(db)
    .WaitFor(db)
    .WithEnvironment("OPENROUTER_API_KEY", openRouterKey)
    .WithEnvironment("OPENAI_API_KEY", openAiKey);

builder.AddProject<Projects.Learning_Client>("client")
    .WithReference(server)
    .WaitFor(server);

builder.Build().Run();
