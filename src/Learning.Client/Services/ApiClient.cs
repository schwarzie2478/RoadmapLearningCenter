using System.Net.Http.Json;
using System.Text.Json;
using Learning.Shared.DTOs;

namespace Learning.Client.Services;

public class ApiClient(HttpClient http)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = null };

    public async Task<RoadmapResponse> GetRoadmapAsync(string? track = null)
    {
        var url = track != null ? $"api/roadmap/{track}" : "api/roadmap";
        return await http.GetFromJsonAsync<RoadmapResponse>(url, JsonOptions) ?? new();
    }

    public async Task<TopicSummaryDto> GetTopicAsync(Guid topicId, string? userId = null)
    {
        var url = userId != null ? $"api/topic/{topicId}?userId={userId}" : $"api/topic/{topicId}";
        return await http.GetFromJsonAsync<TopicSummaryDto>(url, JsonOptions)
            ?? throw new InvalidOperationException($"Topic {topicId} not found.");
    }

    public async Task<TopicBlockDto> GetBlockAsync(Guid topicId, Guid blockId)
    {
        return await http.GetFromJsonAsync<TopicBlockDto>($"api/topic/{topicId}/blocks/{blockId}", JsonOptions)
            ?? throw new InvalidOperationException($"Block {blockId} not found.");
    }

    public async Task<ParagraphDto> AskQuestionAsync(AIAskRequest request)
    {
        var response = await http.PostAsJsonAsync("api/ai/ask", request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ParagraphDto>(JsonOptions))
            ?? throw new InvalidOperationException("No response from AI ask.");
    }

    public async Task<AIGenerateBlockResponse> GenerateBlockAsync(AIGenerateBlockRequest request)
    {
        var response = await http.PostAsJsonAsync("api/ai/generate-block", request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AIGenerateBlockResponse>(JsonOptions))
            ?? throw new InvalidOperationException("No response from generate-block.");
    }

    public async Task<PlaygroundExecuteResponse> ExecuteCodeAsync(PlaygroundExecuteRequest request)
    {
        var response = await http.PostAsJsonAsync("api/playground/execute", request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PlaygroundExecuteResponse>(JsonOptions))
            ?? throw new InvalidOperationException("No response from code execution.");
    }

    public async Task<PlaygroundSessionDto> SaveSessionAsync(PlaygroundSaveRequest request)
    {
        var response = await http.PostAsJsonAsync("api/playground/save", request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PlaygroundSessionDto>(JsonOptions))
            ?? throw new InvalidOperationException("No response from save session.");
    }

    public async Task<ProgressOverviewDto> GetProgressOverviewAsync(string userId)
    {
        return await http.GetFromJsonAsync<ProgressOverviewDto>($"api/progress?userId={userId}", JsonOptions)
            ?? new ProgressOverviewDto();
    }

    public async Task<List<LearnerTopicStateDto>> GetReviewListAsync(string userId)
    {
        return await http.GetFromJsonAsync<List<LearnerTopicStateDto>>($"api/progress/review?userId={userId}", JsonOptions)
            ?? [];
    }

    public async Task<LearnerTopicStateDto?> UpdateProgressAsync(Guid topicId, UpdateProgressRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/progress/{topicId}", request, JsonOptions);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<LearnerTopicStateDto>(JsonOptions)
            : null;
    }
}
