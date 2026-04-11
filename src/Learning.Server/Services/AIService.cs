using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Learning.Server.Data;
using Learning.Shared.Domain;
using Learning.Shared.DTOs;
using Learning.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Learning.Server.Services;

public class AIService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<AIService> _logger;
    private readonly HttpClient _http;
    private readonly string _provider;
    private readonly string _model;

    public AIService(AppDbContext db, IConfiguration config, ILogger<AIService> logger, IHttpClientFactory httpFactory)
    {
        _db = db;
        _config = config;
        _logger = logger;
        _http = httpFactory.CreateClient("AI");

        _provider = config["LLM:Provider"]?.ToLowerInvariant() ?? "openrouter";
        _model = _provider switch
        {
            "openai" => config["LLM:OpenAI:Model"] ?? "gpt-4o",
            _ => config["LLM:OpenRouter:Model"] ?? "qwen/qwen-3.6-plus"
        };

    }

    public async Task<string> GenerateBlockContentAsync(Guid topicId, Guid blockId, CancellationToken ct = default)
    {
        var block = await _db.TopicBlocks
            .Include(b => b.Topic)
            .Include(b => b.Paragraphs)
            .FirstOrDefaultAsync(b => b.Id == blockId && b.TopicId == topicId, ct);

        if (block == null) throw new KeyNotFoundException($"Block {blockId} not found for topic {topicId}.");

        var topic = block.Topic;
        var prereqs = await GetPrerequisiteTitlesAsync(topic.Prerequisites.ToList(), ct);

        var systemPrompt = $"""
You are a .NET tutor explaining "{topic.Title} — {block.Title}".
Block type: {block.Type}
Block seed prompt: {block.SeedPrompt}
Track: {topic.Track} · Difficulty: {topic.Difficulty}
Prerequisites: {string.Join(", ", prereqs.Any() ? prereqs : new[] { "none" })}

Response rules:
- Be concise and focused. Under 400 words.
- Use C# 12+ syntax.
- Include one well-commented code example.
- If Exercise or Quiz, pose a challenge rather than giving the full answer.
- Start with a brief overview.

Formatting guidelines:
- Use ## or ### for section headers.
- Add a blank line between sections and between every bullet point.
- Break dense explanations into short paragraphs (2-3 sentences max).
- Always use bullet points for lists — never inline comma-separated lists.
- Highlight key terms and API names in **bold** on first mention.
- Format code blocks with ```csharp and proper indentation.
- Add brief comments inside code that explain intent, not syntax.
- End with a "### Key Takeaway" section of one or two sentences.
- Never produce walls of text — if a paragraph exceeds ~80 words, split it.

Tone: Educational, clear, and slightly conversational. Assume the reader is learning, not skimming documentation.
""";

        var messages = new[]
        {
            new ChatMsg { Role = "system", Content = systemPrompt },
            new ChatMsg { Role = "user", Content = "Generate the content for this block." }
        };

        var result = await CallLLMAsync(messages, ct);
        _logger.LogInformation("Generated block content for {BlockId} via {Provider}/{Model}", blockId, _provider, _model);
        return result;
    }

    public async Task<ParagraphThread> AnswerQuestionAsync(AIAskRequest request,
        List<ParagraphThread>? parentContext, CancellationToken ct = default)
    {
        var block = await _db.TopicBlocks
            .Include(b => b.Topic)
            .FirstOrDefaultAsync(b => b.Id == request.BlockId && b.TopicId == request.TopicId, ct);

        if (block == null) throw new KeyNotFoundException("Block not found.");

        var topic = block.Topic;
        var prereqs = await GetPrerequisiteTitlesAsync(topic.Prerequisites.ToList(), ct);

        var sbContext = new StringBuilder();
        if (parentContext?.Count > 0)
        {
            sbContext.AppendLine("Thread context so far:");
            foreach (var p in parentContext)
                sbContext.AppendLine($"- [{p.CreatedBy}] {p.Text}");
        }

        var systemPrompt = $"""
You are a .NET tutor answering a question about "{topic.Title} — {block.Title}".
Block type: {block.Type}
Difficulty: {topic.Difficulty}
Prerequisites: {string.Join(", ", prereqs.Any() ? prereqs : new[] { "none" })}
Explanation context: {request.Context}

{sbContext}

Response rules: Keep under 300 words. Use C# 12+ syntax. Include one focused example. End with one reflection question.

Formatting guidelines:
- Use ### for any section headers.
- Break dense text into short paragraphs (2-3 sentences max).
- Always use bullet points for lists.
- Highlight key terms in **bold** on first mention.
- Format code blocks with ```csharp and proper indentation.

Tone: Educational, clear, and slightly conversational. Assume the reader is learning, not skimming documentation.
""";

        var messages = new List<ChatMsg>
        {
            new() { Role = "system", Content = systemPrompt },
            new() { Role = "user", Content = request.Question }
        };

        var answer = await CallLLMAsync(messages, ct);
        _logger.LogInformation("Answered question for block {BlockId} via {Provider}/{Model}", request.BlockId, _provider, _model);

        var paragraph = new ParagraphThread
        {
            Id = Guid.NewGuid(),
            BlockId = request.BlockId,
            ParentParagraphId = request.ParagraphId,
            Text = answer,
            CreatedBy = ContentAuthor.AI,
            Depth = parentContext?.Max(p => p.Depth) + 1 ?? 0,
            CreatedAt = DateTime.UtcNow
        };

        _db.ParagraphThreads.Add(paragraph);
        await _db.SaveChangesAsync(ct);

        return paragraph;
    }


    private async Task<string> CallLLMAsync(IList<ChatMsg> messages, CancellationToken ct)
    {
        var (baseUrl, reqBody) = _provider switch
        {
            "openai" => BuildOpenAIRequest(messages),
            _ => BuildOpenRouterRequest(messages)
        };

        var json = JsonSerializer.Serialize(reqBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/chat/completions")
        {
            Content = content
        };

        if (_provider == "openrouter")
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetOpenRouterKey());
            request.Headers.Add("HTTP-Referer", "https://localhost");
            request.Headers.Add("X-Title", "OhMyPi Learning");
        }
        else
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetOpenAIKey());
        }
        _http.Timeout = TimeSpan.FromSeconds(300);
        using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("LLM call failed with {StatusCode}: {ErrorBody}", response.StatusCode, errorBody);
            throw new HttpRequestException($"LLM call failed with {response.StatusCode}: {errorBody}");
        }
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        // read line-by-line for SSE, or read to end if you just want the full result
        var resultJson = await reader.ReadToEndAsync();
        //var resultJson = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<ChatResp>(resultJson, JsonOpts);
        return result?.Choices?[0]?.Message?.Content?.Trim()
            ?? throw new InvalidOperationException("Empty response from LLM.");
    }

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private (string Url, object Body) BuildOpenAIRequest(IList<ChatMsg> messages)
    {
        return ("https://api.openai.com/v1", new
        {
            model = _model,
            messages,
            max_tokens = 1500,
            temperature = 0.7d
        });
    }
    private (string Url, object Body) BuildOpenRouterRequest(IList<ChatMsg> messages)
    {
        var model = GetConfigValue("LLM:OpenRouter:Model") ?? "qwen/qwen-3.6-plus";
        var maxTokens = GetConfigValueAsInt("LLM:OpenRouter:MaxTokens", 1500);
        var temperature = GetConfigValueAsFloat("LLM:OpenRouter:Temperature", 0.7f);

        return ("https://openrouter.ai/api/v1", new
        {
            model,
            messages,
            max_tokens = maxTokens,
            temperature
        });
    }

    private string GetOpenRouterKey()
    {
        var key = GetConfigValue("LLM:OpenRouter:ApiKey")
            ?? Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("OpenRouter API key not configured. Set LLM__OpenRouter__ApiKey or OPENROUTER_API_KEY.");
        return key;
    }

    private string GetOpenAIKey()
    {
        var key = GetConfigValue("LLM:OpenAI:ApiKey")
            ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("OpenAI API key not configured.");
        return key;
    }

    private string? GetConfigValue(string key) => _config[key];
    private int GetConfigValueAsInt(string key, int fallback) => int.TryParse(_config[key], out var v) ? v : fallback;
    private float GetConfigValueAsFloat(string key, float fallback) => float.TryParse(_config[key], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : fallback;

    private async Task<List<string>> GetPrerequisiteTitlesAsync(List<Guid> prereqIds, CancellationToken ct)
    {
        if (prereqIds.Count == 0) return new();
        return await _db.RoadmapTopics
            .Where(t => prereqIds.Contains(t.Id))
            .Select(t => t.Title)
            .ToListAsync(ct);
    }
}

#pragma warning disable CS8618, CS8601
internal class ChatMsg
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}

internal class ChatResp
{
    [JsonPropertyName("choices")]
    public List<ChatChoice> Choices { get; set; }
}

internal class ChatChoice
{
    [JsonPropertyName("message")]
    public ChatMsg Message { get; set; }
}
#pragma warning restore CS8618, CS8601
