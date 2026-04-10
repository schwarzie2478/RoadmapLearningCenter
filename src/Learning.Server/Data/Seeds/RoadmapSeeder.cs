using Microsoft.EntityFrameworkCore;
using Learning.Server.Data;
using Learning.Shared.Domain;
using Learning.Shared.Enums;

namespace Learning.Server.Data.Seeds;

/// <summary>
/// Seeds initial roadmap topics and blocks on first run.
/// Idempotent — skips if any topic already exists.
/// </summary>
public static class SeedData
{
    public static async Task EnsureSeeded(AppDbContext db, CancellationToken ct = default)
    {
        if (await db.RoadmapTopics.AnyAsync(ct))
            return;

        // --- Foundation Track ---
        var csharpBasics = new RoadmapTopic
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Title = "C# Basics",
            Slug = "csharp-basics",
            Description = "Variables, types, operators, control flow, and methods.",
            Track = RoadmapTrack.Foundation,
            Order = 1,
            Difficulty = Difficulty.Beginner,
            Blocks = new List<TopicBlock>
            {
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000101"), Title = "Variables & Types", Type = BlockType.Intro, SeedPrompt = "Explain variables, value vs reference types, and the primitive types in C#.", Order = 1 },
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000102"), Title = "Control Flow", Type = BlockType.Explanation, SeedPrompt = "Explain if/else, switch expressions, pattern matching, loops, and early returns.", Order = 2 },
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000103"), Title = "Methods", Type = BlockType.Example, SeedPrompt = "Show method syntax, parameters, return types, ref/out/in, optional params, expression-bodied members.", Order = 3 },
            }
        };

        var oop = new RoadmapTopic
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Title = "Object-Oriented Programming",
            Slug = "oop",
            Description = "Classes, interfaces, inheritance, polymorphism, and encapsulation.",
            Track = RoadmapTrack.Foundation,
            Order = 2,
            Difficulty = Difficulty.Beginner,
            Prerequisites = new List<Guid> { csharpBasics.Id },
            Blocks = new List<TopicBlock>
            {
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000201"), Title = "Classes & Objects", Type = BlockType.Explanation, SeedPrompt = "Explain classes, constructors, fields, properties (init, required), records, and structural equality.", Order = 1 },
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000202"), Title = "Inheritance & Polymorphism", Type = BlockType.Explanation, SeedPrompt = "Explain base classes, virtual/override, abstract classes, sealed, and interface default implementations.", Order = 2 },
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000203"), Title = "Interfaces", Type = BlockType.Example, SeedPrompt = "Show interface design, explicit implementation, marker interfaces, multiple inheritance vs C# single.", Order = 3 },
            }
        };

        // --- Backend Track ---
        var aspnetCoreBasics = new RoadmapTopic
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
            Title = "ASP.NET Core Basics",
            Slug = "aspnet-core-basics",
            Description = "Minimal APIs, controllers, middleware, dependency injection, and configuration.",
            Track = RoadmapTrack.Backend,
            Order = 1,
            Difficulty = Difficulty.Beginner,
            Prerequisites = new List<Guid> { csharpBasics.Id, oop.Id },
            Blocks = new List<TopicBlock>
            {
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000301"), Title = "Minimal APIs", Type = BlockType.Intro, SeedPrompt = "Explain IApplicationBuilder, WebApplication builder, route groups, and endpoint conventions.", Order = 1 },
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000302"), Title = "Dependency Injection", Type = BlockType.Explanation, SeedPrompt = "Explain AddScoped, AddTransient, AddSingleton, IServiceCollection, and factory patterns.", Order = 2 },
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000303"), Title = "Middleware Pipeline", Type = BlockType.Explanation, SeedPrompt = "Explain Use, Run, Map, ordering, short-circuiting, and custom middleware components.", Order = 3 },
            }
        };

        var efCore = new RoadmapTopic
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
            Title = "Entity Framework Core",
            Slug = "ef-core",
            Description = "DbContext, migrations, LINQ queries, relationships, and performance.",
            Track = RoadmapTrack.Backend,
            Order = 2,
            Difficulty = Difficulty.Intermediate,
            Prerequisites = new List<Guid> { aspnetCoreBasics.Id },
            Blocks = new List<TopicBlock>
            {
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000401"), Title = "DbContext & Models", Type = BlockType.Explanation, SeedPrompt = "Explain DbSet, OnConfiguring, Fluent API, Value conversions, and seed data.", Order = 1 },
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000402"), Title = "Migrations", Type = BlockType.Explanation, SeedPrompt = "Explain add-migration, update-database, scripting, reverting, and production deployment.", Order = 2 },
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000403"), Title = "Query Performance", Type = BlockType.Explanation, SeedPrompt = "Explain Include vs Select, explicit loading, split queries, compiled queries, and batched updates.", Order = 3 },
            }
        };

        // --- Blazor Track ---
        var blazorComponents = new RoadmapTopic
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
            Title = "Blazor Components",
            Slug = "blazor-components",
            Description = "Component model, parameters, render fragments, and event callbacks.",
            Track = RoadmapTrack.Blazor,
            Order = 1,
            Difficulty = Difficulty.Beginner,
            Prerequisites = new List<Guid> { aspnetCoreBasics.Id },
            Blocks = new List<TopicBlock>
            {
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000501"), Title = "Component Basics", Type = BlockType.Intro, SeedPrompt = "Explain Razor component syntax, parameters, component lifecycle, and StateHasChanged.", Order = 1 },
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000502"), Title = "Event Callbacks", Type = BlockType.Example, SeedPrompt = "Explain EventCallback vs Action, two-way binding, cascading values, and render tree diff.", Order = 2 },
            }
        };

        // --- FullStack & Senior (placeholders) ---
        var fullStackProject = new RoadmapTopic
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
            Title = "Full-Stack Project",
            Slug = "fullstack-project",
            Description = "Build a complete application: API, database, WASM client, deployment.",
            Track = RoadmapTrack.FullStack,
            Order = 1,
            Difficulty = Difficulty.Advanced,
            Prerequisites = new List<Guid> { efCore.Id, blazorComponents.Id },
            Blocks = new List<TopicBlock>
            {
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000601"), Title = "Architecture", Type = BlockType.Intro, SeedPrompt = "Explain clean architecture, shared DTOs, project references, and deployment strategies.", Order = 1 },
            }
        };

        var seniorPatterns = new RoadmapTopic
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
            Title = "Advanced .NET Patterns",
            Slug = "advanced-dotnet-patterns",
            Description = "Pipeline architecture, MediatR, CQRS, domain-driven design, and resilience.",
            Track = RoadmapTrack.Senior,
            Order = 1,
            Difficulty = Difficulty.Advanced,
            Prerequisites = new List<Guid> { fullStackProject.Id },
            Blocks = new List<TopicBlock>
            {
                new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000701"), Title = "Pipeline & CQRS", Type = BlockType.Explanation, SeedPrompt = "Explain pipeline behavior, MediatR interfaces, command/query separation, and cross-cutting concerns.", Order = 1 },
            }
        };

        var allTopics = new[]
        {
            csharpBasics, oop, aspnetCoreBasics, efCore, blazorComponents, fullStackProject, seniorPatterns
        };

        db.RoadmapTopics.AddRange(allTopics);
        await db.SaveChangesAsync(ct);
    }
}
