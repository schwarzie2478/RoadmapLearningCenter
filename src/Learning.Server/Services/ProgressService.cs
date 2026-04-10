using Learning.Server.Data;
using Learning.Shared.DTOs;
using Learning.Shared.Domain;
using Learning.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Learning.Server.Services;

/// <summary>
/// Manages learner topic state: progress, confidence, and review flags.
/// </summary>
public class ProgressService
{
    private readonly AppDbContext _db;

    public ProgressService(AppDbContext db) => _db = db;

    public async Task<ProgressOverviewDto> GetOverviewAsync(string userId, CancellationToken ct = default)
    {
        var states = await _db.LearnerTopicStates
            .Where(s => s.UserId == userId)
            .ToListAsync(ct);

        var topicIds = states.Select(s => s.TopicId).ToList();
        var titles = await _db.RoadmapTopics
            .Where(t => topicIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.Title, ct);

        var stateDtos = states.Select(s => new LearnerTopicStateDto
        {
            TopicId = s.TopicId,
            TopicTitle = titles.GetValueOrDefault(s.TopicId, "Unknown"),
            Status = s.Status,
            Confidence = s.Confidence,
            LastVisitedAt = s.LastVisitedAt,
            NeedsReview = s.NeedsReview,
            CompletedAt = s.CompletedAt
        }).ToList();

        // Also include topics from roadmap that haven't been visited yet
        var allTopicIds = await _db.RoadmapTopics.Select(t => t.Id).ToListAsync(ct);
        foreach (var id in allTopicIds)
        {
            if (!stateDtos.Any(s => s.TopicId == id))
            {
                var title = await _db.RoadmapTopics.Where(t => t.Id == id).Select(t => t.Title).FirstOrDefaultAsync(ct);
                stateDtos.Add(new LearnerTopicStateDto
                {
                    TopicId = id,
                    TopicTitle = title ?? "Unknown",
                    Status = TopicStatus.NotStarted,
                    Confidence = 0,
                    LastVisitedAt = DateTime.UtcNow
                });
            }
        }

        return new ProgressOverviewDto
        {
            TopicStates = stateDtos,
            OverallConfidence = stateDtos.Any() ? stateDtos.Average(s => s.Confidence) : 0,
            TopicsCompleted = stateDtos.Count(s => s.Status == TopicStatus.Done),
            TopicsExploring = stateDtos.Count(s => s.Status == TopicStatus.Exploring)
        };
    }

    public async Task<LearnerTopicStateDto?> UpdateStateAsync(string userId, Guid topicId,
        UpdateProgressRequest request, CancellationToken ct = default)
    {
        var state = await _db.LearnerTopicStates
            .FirstOrDefaultAsync(s => s.UserId == userId && s.TopicId == topicId, ct);

        if (state == null)
        {
            state = new LearnerTopicState { UserId = userId, TopicId = topicId };
            _db.LearnerTopicStates.Add(state);
        }

        if (request.Status.HasValue)
            state.Status = request.Status.Value;

        if (request.Confidence.HasValue)
            state.Confidence = request.Confidence.Value;

        state.LastVisitedAt = DateTime.UtcNow;

        if (request.Status == TopicStatus.Done && state.CompletedAt == null)
            state.CompletedAt = DateTime.UtcNow;

        if (request.Confidence is < 40 and > 0)
            state.NeedsReview = true;
        else if (request.Status == TopicStatus.Done)
            state.NeedsReview = false;

        await _db.SaveChangesAsync(ct);

        var title = await _db.RoadmapTopics.Where(t => t.Id == topicId).Select(t => t.Title).FirstOrDefaultAsync(ct);

        return new LearnerTopicStateDto
        {
            TopicId = state.TopicId,
            TopicTitle = title ?? "Unknown",
            Status = state.Status,
            Confidence = state.Confidence,
            LastVisitedAt = state.LastVisitedAt,
            NeedsReview = state.NeedsReview,
            CompletedAt = state.CompletedAt
        };
    }

    public async Task<List<LearnerTopicStateDto>> GetReviewListAsync(string userId, CancellationToken ct = default)
    {
        var states = await _db.LearnerTopicStates
            .Where(s => s.UserId == userId && s.NeedsReview)
            .ToListAsync(ct);

        var topicIds = states.Select(s => s.TopicId).ToList();
        var titles = await _db.RoadmapTopics
            .Where(t => topicIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.Title, ct);

        return states.Select(s => new LearnerTopicStateDto
        {
            TopicId = s.TopicId,
            TopicTitle = titles.GetValueOrDefault(s.TopicId, "Unknown"),
            Status = s.Status,
            Confidence = s.Confidence,
            LastVisitedAt = s.LastVisitedAt,
            NeedsReview = s.NeedsReview,
            CompletedAt = s.CompletedAt
        }).ToList();
    }
}
