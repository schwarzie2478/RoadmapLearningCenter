# ASP.NET Core Developer Roadmap — 21 Child Topics

## Parent: ASP.NET Core Basics
- **Id**: `10000000-0000-0000-0000-000000000003`
- **Track**: Backend · **Difficulty**: Beginner
- **Prerequisites**: C# Basics, OOP
- **Description**: Minimal APIs, controllers, middleware, dependency injection, and configuration.

---

## 1. General Development Skills *(Foundation · Beginner)*
- **Id**: `10000000-0000-0000-0000-000000000008`
- **Prerequisites**: none
- **Description**: Git, HTTP basics, TLS/SSL, search skills, algorithms, AI/LLM tools.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000000801` — Git & HTTP *(Intro)* — Git basics, HTTP methods (GET/POST/PUT/DELETE/OPTIONS), request/response lifecycle.
  2. `20000000-0000-0000-0000-000000000802` — TLS/SSL & Security Basics *(Explanation)* — What TLS and SSL are, certificate management, HTTPS enforcement in ASP.NET Core.
  3. `20000000-0000-0000-0000-000000000803` — AI & LLM Tools *(Explanation)* — Using ChatGPT/Claude/Gemini for development, Cursor AI features, MCPs, Semantic Kernel, OpenAI .NET SDK.

## 2. C# *(Foundation · Beginner)*
- **Id**: `10000000-0000-0000-0000-000000000009`
- **Prerequisites**: C# Basics
- **Description**: Advanced C# language features, .NET CLI, and code quality.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000000901` — Advanced C# Features *(Explanation)* — Records, pattern matching, nullable reference types, spans, ref structs, source generators.
  2. `20000000-0000-0000-0000-000000000902` — .NET CLI & Tooling *(Example)* — dotnet new, build, publish, tool manifest (.NET tools), global.json, NuGet management.
  3. `20000000-0000-0000-0000-000000000903` — Code Quality *(Explanation)* — StyleCop rules, analyzers, editorconfig, Roslyn source generators, nullable context.

## 3. SQL Fundamentals *(Backend · Beginner)*
- **Id**: `10000000-0000-0000-0000-000000000010`
- **Prerequisites**: none
- **Description**: T-SQL querying fundamentals, joins, indexes, and stored procedures.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000000a01` — T-SQL Basics *(Intro)* — SELECT, FROM, WHERE, ORDER BY, GROUP BY, HAVING. Query execution order.
  2. `20000000-0000-0000-0000-000000000a02` — Joins & Subqueries *(Explanation)* — INNER, LEFT, RIGHT, FULL JOINs. EXISTS, IN, correlated subqueries, CTEs, window functions.
  3. `20000000-0000-0000-0000-000000000a03` — Indexes & Performance *(Example)* — Clustered vs non-clustered indexes, execution plans, covering indexes, query optimization.

## 4. SOLID *(Backend · Beginner)*
- **Id**: `10000000-0000-0000-0000-000000000011`
- **Prerequisites**: ASP.NET Core Basics, OOP
- **Description**: Five design principles for maintainable, extensible object-oriented code.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000000b01` — SRP & OCP *(Explanation)* — Single Responsibility and Open-Closed Principles with C# examples.
  2. `20000000-0000-0000-0000-000000000b02` — LSP, ISP & DIP *(Explanation)* — Liskov Substitution, Interface Segregation, and Dependency Inversion with real violations and fixes.

## 5. ORM *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000012`
- **Prerequisites**: SQL Fundamentals, ASP.NET Core Basics
- **Description**: EF Core as primary ORM, Dapper for micro-ORM scenarios.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000000c01` — EF Core Fundamentals *(Explanation)* — DbContext, entity configuration, Fluent API, migrations, querying.
  2. `20000000-0000-0000-0000-000000000c02` — Dapper *(Example)* — Raw SQL performance, POCO mapping, when to choose Dapper over EF Core.

## 6. Dependency Injection *(Backend · Beginner)*
- **Id**: `10000000-0000-0000-0000-000000000013`
- **Prerequisites**: ASP.NET Core Basics
- **Description**: Service lifetimes, DI containers, registration patterns.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000000d01` — Service Lifetimes *(Intro)* — Transient, Scoped, Singleton with concrete examples and common capture bugs.
  2. `20000000-0000-0000-0000-000000000d02` — DI Containers *(Explanation)* — Microsoft.Extensions.DependencyInjection, AutoFac integration, factory patterns.
  3. `20000000-0000-0000-0000-000000000d03` — Scrutor & Conventions *(Example)* — Assembly scanning, decoration, convention-based registration.

## 7. Databases *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000014`
- **Prerequisites**: SQL Fundamentals, ASP.NET Core Basics
- **Description**: Relational databases, search engines, and NoSQL stores.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000000e01` — Relational Databases *(Intro)* — SQL Server, PostgreSQL, MySQL, MariaDB comparison.
  2. `20000000-0000-0000-0000-000000000e02` — Search Engines *(Explanation)* — ElasticSearch, Meilisearch, OpenSearch, ManticoreSearch for full-text search.
  3. `20000000-0000-0000-0000-000000000e03` — NoSQL Stores *(Explanation)* — Redis, MongoDB, Cassandra, LiteDB, RavenDB, CouchDB, CosmosDB, DynamoDB. CAP trade-offs.

## 8. Caching *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000015`
- **Prerequisites**: ASP.NET Core Basics
- **Description**: In-memory, distributed, response, and output caching strategies.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000000f01` — Memory & Distributed Cache *(Intro)* — IMemoryCache vs IDistributedCache, Redis (StackExchange.Redis, EasyCaching), Memcached.
  2. `20000000-0000-0000-0000-000000000f02` — Response & Output Caching *(Explanation)* — Built-in response caching, Marvin.Cache.Headers, output caching middleware, EF Core 2nd-level cache.

## 9. Log Frameworks *(Backend · Beginner)*
- **Id**: `10000000-0000-0000-0000-000000000016`
- **Prerequisites**: ASP.NET Core Basics
- **Description**: Structured logging with Serilog and NLog.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001001` — Structured Logging *(Intro)* — ILogger, Serilog sinks, enrichers, structured properties.
  2. `20000000-0000-0000-0000-000000001002` — Serilog & NLog *(Explanation)* — Compare configuration, sinks (Console, File, Seq, Elasticsearch), performance.

## 10. API Clients & Communications *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000017`
- **Prerequisites**: ASP.NET Core Basics
- **Description**: REST patterns, gRPC, and GraphQL.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001101` — REST Patterns *(Intro)* — Gridify filtering, OData, REPR pattern with Minimal APIs, FastEndpoints.
  2. `20000000-0000-0000-0000-000000001102` — gRPC *(Explanation)* — Protobuf contracts, ASP.NET Core gRPC server/client, unary vs streaming.
  3. `20000000-0000-0000-0000-000000001103` — GraphQL *(Explanation)* — HotChocolate vs GraphQL-dotnet, schema design, DataLoader, N+1 problem.

## 11. Real-Time Communication *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000018`
- **Prerequisites**: ASP.NET Core Basics
- **Description**: SignalR and WebSockets for bidirectional communication.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001201` — SignalR *(Intro)* — Hubs, clients, groups, reconnection, transport fallback.
  2. `20000000-0000-0000-0000-000000001202` — WebSockets *(Explanation)* — Raw WebSocket middleware, message framing, vs SignalR trade-offs.

## 12. Object Mapping *(Backend · Beginner)*
- **Id**: `10000000-0000-0000-0000-000000000019`
- **Prerequisites**: C# Basics
- **Description**: Converting between domain models and DTOs.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001301` — Manual Mapping *(Intro)* — Why manual mapping is often best — compile-time safety, no reflection cost.
  2. `20000000-0000-0000-0000-000000001302` — Mapperly & AutoMapper *(Explanation)* — Mapperly compile-time source gen, AutoMapper projections and pitfalls.

## 13. Background Task Scheduler *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000020`
- **Prerequisites**: ASP.NET Core Basics
- **Description**: Background services, schedulers, recurring job processors.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001401` — IHostedService & BackgroundService *(Intro)* — Native hosted services, graceful shutdown, cancellation tokens.
  2. `20000000-0000-0000-0000-000000001402` — Hangfire & Quartz *(Explanation)* — Hangfire dashboards, recurring jobs. Quartz scheduling, job stores, clustering.
  3. `20000000-0000-0000-0000-000000001403` — Coravel *(Example)* — Coravel scheduler, event queuing, task orchestration for simpler apps.

## 14. Testing *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000021`
- **Prerequisites**: ASP.NET Core Basics, C# Basics
- **Description**: Unit, integration, snapshot, behavior, E2E, performance, and architecture testing.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001501` — Unit Testing *(Intro)* — xUnit/NUnit/MSTest, Moq/NSubstitute, FluentAssertions, Bogus, AutoFixture.
  2. `20000000-0000-0000-0000-000000001502` — Integration & E2E *(Explanation)* — WebApplicationFactory, test containers, Respawn, Selenium, Puppeteer-Sharp.
  3. `20000000-0000-0000-0000-000000001503` — Performance & Architecture *(Example)* — K6, Crank, Bombardier, ArchUnitNET, NetArchTest.

## 15. Microservices *(Backend · Advanced)*
- **Id**: `10000000-0000-0000-0000-000000000022`
- **Prerequisites**: Databases, Testing, ASP.NET Core Basics
- **Description**: Message brokers, bus, gateways, containers, orchestration.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001601` — Message Brokers & Bus *(Intro)* — RabbitMQ, Kafka, Azure Service Bus, SQS, NetMQ. MassTransit, NServiceBus, EasyNetQ.
  2. `20000000-0000-0000-0000-000000001602` — API Gateway & Containerization *(Explanation)* — Ocelot, YARP, Docker, Podman.
  3. `20000000-0000-0000-0000-000000001603` — Orchestration & Actor Models *(Explanation)* — Kubernetes, .NET Aspire, Orleans, Proto.Actor, Dapr, Akka.NET.

## 16. CI/CD *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000023`
- **Prerequisites**: Testing, ASP.NET Core Basics
- **Description**: Automated build, test, and deployment pipelines.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001701` — CI/CD Pipelines *(Intro)* — GitHub Actions for .NET, Azure Pipelines, GitLab CI/CD, TeamCity.

## 17. Design Patterns *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000024`
- **Prerequisites**: SOLID, OOP
- **Description**: Creational, structural, and behavioral patterns with C# implementations.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001801` — Creational Patterns *(Explanation)* — Singleton, Factory, Abstract Factory, Builder, Prototype. When and why in .NET.
  2. `20000000-0000-0000-0000-000000001802` — Structural & Behavioral Patterns *(Explanation)* — Adapter, Decorator, Observer, Strategy, Command, Chain of Responsibility. .NET examples.

## 18. Monitoring/Logging/Tracing/Alerting *(Backend · Intermediate)*
- **Id**: `10000000-0000-0000-0000-000000000025`
- **Prerequisites**: Log Frameworks, ASP.NET Core Basics
- **Description**: Metrics, tracing, error tracking, and alerting — on-premises and cloud.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001901` — Metrics *(Intro)* — Prometheus + Grafana on-premises, Datadog cloud. .NET metrics export, custom counters.
  2. `20000000-0000-0000-0000-000000001902` — Logging Aggregation *(Explanation)* — ELK Stack, Seq, Sentry.io. Datadog logs.
  3. `20000000-0000-0000-0000-000000001903` — Tracing & Alerting *(Example)* — OpenTelemetry, Jaeger, Zipkin. Zabbix, Alertmanager, Datadog monitors.

## 19. Client-Side .NET *(Backend · Beginner)*
- **Id**: `10000000-0000-0000-0000-000000000026`
- **Prerequisites**: ASP.NET Core Basics
- **Description**: Template engines and client-side .NET frameworks.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001a01` — Template Engines *(Intro)* — Razor engine, Scriban, Fluid — server-side templating vs API responses.
  2. `20000000-0000-0000-0000-000000001a02` — Frameworks *(Explanation)* — Blazor WebAssembly/Server, .NET MAUI for desktop/mobile.

## 20. Good to Know *(Backend · Beginner)*
- **Id**: `10000000-0000-0000-0000-000000000027`
- **Prerequisites**: ASP.NET Core Basics
- **Description**: Essential libraries and tools.
- **Blocks**:
  1. `20000000-0000-0000-0000-000000001b01` — Essential Libraries *(Intro)* — Scalar, MediatR, FluentValidation, Polly, BenchmarkDotNet.
  2. `20000000-0000-0000-0000-000000001b02` — Advanced Tools *(Explanation)* — DistributedLock, EF Core Bulk Extensions, Nuke Build, Marten.
