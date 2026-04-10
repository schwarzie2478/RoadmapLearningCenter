using System.ComponentModel.DataAnnotations;
using Learning.Shared.Enums;

namespace Learning.Shared.DTOs;

/// <summary>
/// Represents a roadmap topic in responses; includes children for tree rendering.
/// </summary>
public class RoadmapTopicDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RoadmapTrack Track { get; set; }
    public int Order { get; set; }
    public Difficulty Difficulty { get; set; }
    public IReadOnlyList<Guid> Prerequisites { get; set; } = Array.Empty<Guid>();
    public IReadOnlyList<RoadmapTopicDto> Children { get; set; } = Array.Empty<RoadmapTopicDto>();
}

/// <summary>
/// Root wrapper for roadmap responses.
/// </summary>
public class RoadmapResponse
{
    public IReadOnlyList<RoadmapTopicDto> Topics { get; set; } = Array.Empty<RoadmapTopicDto>();
}
