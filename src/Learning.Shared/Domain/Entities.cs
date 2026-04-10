using System.ComponentModel.DataAnnotations;
using Learning.Shared.Enums;

namespace Learning.Shared.Domain;

/// <summary>
/// A node in the learning roadmap tree.
/// </summary>
public class RoadmapTopic
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }
    public RoadmapTopic? Parent { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public RoadmapTrack Track { get; set; }

    public int Order { get; set; }

    public Difficulty Difficulty { get; set; }

    public ICollection<Guid> Prerequisites { get; set; } = new List<Guid>();

    public ICollection<RoadmapTopic> Children { get; set; } = new List<RoadmapTopic>();

    public ICollection<TopicBlock> Blocks { get; set; } = new List<TopicBlock>();
}

/// <summary>
/// An individual learning unit within a roadmap topic.
/// </summary>
public class TopicBlock
{
    public Guid Id { get; set; }

    public Guid TopicId { get; set; }
    public RoadmapTopic Topic { get; set; } = null!;

    public BlockType Type { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string SeedPrompt { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool IsGeneratedOnDemand { get; set; } = true;

    public ICollection<ParagraphThread> Paragraphs { get; set; } = new List<ParagraphThread>();
}

/// <summary>
/// A paragraph in an expandable Q&A thread under a block.
/// </summary>
public class ParagraphThread
{
    public Guid Id { get; set; }

    public Guid BlockId { get; set; }
    public TopicBlock Block { get; set; } = null!;

    public Guid? ParentParagraphId { get; set; }
    public ParagraphThread? ParentParagraph { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    public ContentAuthor CreatedBy { get; set; }

    public int Depth { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ParagraphThread> Replies { get; set; } = new List<ParagraphThread>();
}

/// <summary>
/// Tracks learner's progress through individual topics.
/// </summary>
public class LearnerTopicState
{
    [Required, MaxLength(256)]
    public string UserId { get; set; } = string.Empty;

    public Guid TopicId { get; set; }

    public TopicStatus Status { get; set; } = TopicStatus.NotStarted;

    public float Confidence { get; set; }

    public DateTime LastVisitedAt { get; set; } = DateTime.UtcNow;

    public bool NeedsReview { get; set; }

    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// A saved code playground session.
/// </summary>
public class PlaygroundSession
{
    public Guid Id { get; set; }

    [Required, MaxLength(256)]
    public string UserId { get; set; } = string.Empty;

    public Guid TopicId { get; set; }

    [MaxLength(20)]
    public string Language { get; set; } = "csharp";

    [Required]
    public string Code { get; set; } = string.Empty;

    [MaxLength(10000)]
    public string? Output { get; set; }

    [MaxLength(5000)]
    public string? Errors { get; set; }

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}
