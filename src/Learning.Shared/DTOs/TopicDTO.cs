using System.ComponentModel.DataAnnotations;
using Learning.Shared.Enums;

namespace Learning.Shared.DTOs;

/// <summary>
/// Header view of a topic — lists blocks without content.
/// </summary>
public class TopicSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RoadmapTrack Track { get; set; }
    public Difficulty Difficulty { get; set; }
    public IReadOnlyList<BlockHeaderDto> Blocks { get; set; } = Array.Empty<BlockHeaderDto>();
    public TopicStatus? LearnerStatus { get; set; }
    public float? LearnerConfidence { get; set; }
}

/// <summary>
/// Lightweight header for a single block inside a topic.
/// </summary>
public class BlockHeaderDto
{
    public Guid Id { get; set; }
    public BlockType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
}

/// <summary>
/// Expanded block detail with generated content and paragraph threads.
/// </summary>
public class TopicBlockDto
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public BlockType Type { get; set; }
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// AI-generated explanation for this block.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Paragraph threads attached to this block (root-level only).
    /// </summary>
    public IReadOnlyList<ParagraphDto> Paragraphs { get; set; } = Array.Empty<ParagraphDto>();

    public string? StarterCode { get; set; }
}

/// <summary>
/// A paragraph in a topic block's Q&A thread.
/// </summary>
public class ParagraphDto
{
    public Guid Id { get; set; }
    public Guid BlockId { get; set; }
    public Guid? ParentParagraphId { get; set; }
    public string Text { get; set; } = string.Empty;
    public ContentAuthor CreatedBy { get; set; }
    public int Depth { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<ParagraphDto> Replies { get; set; } = Array.Empty<ParagraphDto>();
}
