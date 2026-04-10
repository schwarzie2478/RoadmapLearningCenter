using Learning.Shared.Enums;

namespace Learning.Shared.DTOs;

/// <summary>
/// Summary of learner progress across topics.
/// </summary>
public class ProgressOverviewDto
{
    public IReadOnlyList<LearnerTopicStateDto> TopicStates { get; set; } = Array.Empty<LearnerTopicStateDto>();
    public float OverallConfidence { get; set; }
    public int TopicsCompleted { get; set; }
    public int TopicsExploring { get; set; }
}

/// <summary>
/// Per-topic learner state in progress responses.
/// </summary>
public class LearnerTopicStateDto
{
    public Guid TopicId { get; set; }
    public string TopicTitle { get; set; } = string.Empty;
    public TopicStatus Status { get; set; }
    public float Confidence { get; set; }
    public DateTime LastVisitedAt { get; set; }
    public bool NeedsReview { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Request to update a topic's progress.
/// </summary>
public class UpdateProgressRequest
{
    public TopicStatus? Status { get; set; }
    public float? Confidence { get; set; }
    public string UserId { get; set; } = string.Empty;
}
