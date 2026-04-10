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
| Database | PostgreSQL (Docker, with pgvector ready) |
| AI | OpenRouter / OpenAI (configurable via `LLM:` keys) |
| Code Execution | Temporary isolated .NET console projects with security scanning |

## Quick Start

Three scripts handle startup. Run in separate terminals, in order:

```powershell
cd scripts

# 1. PostgreSQL in Docker (tensorchord/pgvecto-rs:pg14)
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass; .\Start-Database.ps1

# 2. Backend on http://localhost:5200 (Swagger at /swagger)
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass; .\Start-Backend.ps1

# 3. Frontend on http://localhost:5035
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass; .\Start-Frontend.ps1
```

Then browse to **http://localhost:5035**.

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

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- PostgreSQL image (`tensorchord/pgvecto-rs:pg14-v0.2.0` or any `postgres:14+`)

Optional for AI features — set one of:
- `LLM__OpenRouter__ApiKey` environment variable
- `LLM__OpenAI__ApiKey` environment variable

Without a key, the roadmap, progress tracking, and code playground still work. AI-generated explanations and Q&A will fail at runtime.

## Data Model

- **RoadmapTopic** — hierarchical topic nodes with prerequisites, track, difficulty
- **TopicBlock** — expandable learning units per topic
- **ParagraphThread** — threaded Q&A with parent/child nesting
- **LearnerTopicState** — per-user status, confidence, review flags
- **PlaygroundSession** — saved code + outputs per topic
