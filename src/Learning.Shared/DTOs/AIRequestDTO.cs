using System.ComponentModel.DataAnnotations;
using Learning.Shared.Enums;

namespace Learning.Shared.DTOs;

/// <summary>
/// Request to ask a context-threaded question about a topic.
/// </summary>
public class AIAskRequest
{
    [Required]
    public Guid TopicId { get; set; }

    [Required]
    public Guid BlockId { get; set; }

    public Guid? ParagraphId { get; set; }

    [Required, MaxLength(2000)]
    public string Question { get; set; } = string.Empty;

    public ExplanationContext Context { get; set; } = ExplanationContext.Short;

    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// Response containing an AI-generated answer.
/// </summary>
public class AIAskResponse
{
    public ParagraphDto Paragraph { get; set; } = null!;
}

/// <summary>
/// Request to generate block content on demand.
/// </summary>
public class AIGenerateBlockRequest
{
    [Required]
    public Guid TopicId { get; set; }

    [Required]
    public Guid BlockId { get; set; }

    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// Response with generated block content.
/// </summary>
public class AIGenerateBlockResponse
{
    public TopicBlockDto Block { get; set; } = null!;
}
