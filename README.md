# Roadmap Learning Center

An AI-powered, roadmap-first learning platform for mastering .NET. Not a chatbot, not a static course — a structured curriculum where depth is generated on demand.

## Core Idea

The learning map comes first. You pick a topic, click blocks only when ready, and ask contextual questions that stay anchored to the paragraph you're reading.

### How It Works

1. **Roadmap** — browse topics grouped by track (Foundation, Backend, Blazor, Full-Stack, Senior) with status tracking and prerequisites.
2. **Topic Blocks** — each topic contains lazy-loaded blocks (Intro, Explanation, Example, Exercise, Quiz). Click **Explore** to generate content only when you need it.
3. **Threaded Q&A** — every paragraph is a conversation root. Ask questions, switch between short/deep/analogy explanations, and follow-up replies stay nested in context.
4. **Code Playground** — run C# snippets in a sandboxed environment. Starter code provided per topic, output and errors captured inline.
5. **Review Mode** — surface weak areas: low-confidence topics, repeated questions, failed quiz attempts, topics reopened multiple times.

## Stack

| Layer | Technology |
|---|---|
| Frontend | Blazor WebAssembly |
| Backend | ASP.NET Core Web API |
| Database | PostgreSQL (Docker via Aspire, with pgvector ready) |
| AI | OpenRouter / OpenAI (configurable via `LLM:` keys) |
| Code Execution | Temporary isolated .NET console projects with security scanning |
| Orchestration | .NET Aspire (Dashboard at http://localhost:15888) |

## Quick Start

A single command starts everything (PostgreSQL, Server, Client, Aspire Dashboard):

```powershell
# Starts all components; Dashboard opens automatically at http://localhost:15888
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass; .\scripts\Start-All.ps1
```

Or run the AppHost directly:

```powershell
dotnet run --project src/Learning.AppHost
```

Then browse to the Client at the URL shown in the Aspire Dashboard, or the Dashboard itself at **http://localhost:15888**.

> **Standalone mode** (without Aspire) — run each script in a separate terminal:
>
> ```powershell
> .\scripts\Start-Database.ps1   # PostgreSQL in Docker
> .\scripts\Start-Backend.ps1    # Backend on http://localhost:5200
> .\scripts\Start-Frontend.ps1   # Frontend on http://localhost:5035
> ```

## Aspire Dashboard

The .NET Aspire Dashboard gives you a live view of all running components:

- **Service health** — real-time status of Server, Client, and PostgreSQL
- **Logs** — aggregated, structured logs from every component in one place
- **Traces** — OpenTelemetry distributed traces from browser → API → database
- **Metrics** — request rates, latencies, and resource usage per service

No separate setup needed — the dashboard launches automatically with `Start-All.ps1` and sends telemetry from any service that references `Learning.ServiceDefaults`.

## Initial Curriculum

| Track | Topics |
|---|---|
| Foundation | C# Basics, Object-Oriented Programming |
| Backend | ASP.NET Core Basics, Entity Framework Core |
| Blazor | Blazor Components |
| Full-Stack | Full-Stack Project |
| Senior | Advanced .NET Patterns (Pipeline & CQRS) |

Each topic ships with 1–3 learning blocks and seed prompts for AI-generated explanations.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET Aspire workload](https://learn.microsoft.com/dotnet/aspire/) (`dotnet workload install aspire`)

Optional for AI features — set one of:
- `LLM__OpenRouter__ApiKey` environment variable
- `LLM__OpenAI__ApiKey` environment variable

Without a key, the roadmap, progress tracking, and code playground still work. AI-generated explanations and Q&A will fail at runtime.

## Project Structure

```
src/
  Learning.AppHost/      — Aspire orchestrator (entry point)
  Learning.ServiceDefaults/  — shared OpenTelemetry, health checks, resilience
  Learning.Server/       — ASP.NET Core API
  Learning.Client/       — Blazor WebAssembly frontend
  Learning.Shared/       — shared types and models
scripts/
  Start-All.ps1          — starts everything via Aspire
  Start-Database.ps1     — PostgreSQL only (standalone)
  Start-Backend.ps1      — Server only (standalone)
  Start-Frontend.ps1     — Client only (standalone)
```

## Data Model

- **RoadmapTopic** — hierarchical topic nodes with prerequisites, track, difficulty
- **TopicBlock** — expandable learning units per topic
- **ParagraphThread** — threaded Q&A with parent/child nesting
- **LearnerTopicState** — per-user status, confidence, review flags
- **PlaygroundSession** — saved code + outputs per topic
