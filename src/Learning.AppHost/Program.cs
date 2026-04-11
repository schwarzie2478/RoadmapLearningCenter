var builder = DistributedApplication.CreateBuilder(args);

// Use the existing PostgreSQL on localhost:5432 instead of spinning up a separate container.
// This keeps one database that can be inspected with any management tool.
var db = builder.AddConnectionString("learning");

var openRouterKey = builder.AddParameter("openrouter-api-key", secret: true);
var openAiKey = builder.AddParameter("openai-api-key", secret: true);

var server = builder
    .AddProject<Projects.Learning_Server>("server")
    .WithReference(db)
    .WithEnvironment("OPENROUTER_API_KEY", openRouterKey)
    .WithEnvironment("OPENAI_API_KEY", openAiKey);

builder.AddProject<Projects.Learning_Client>("client")
    .WithReference(server)
    .WaitFor(server);

builder.Build().Run();
