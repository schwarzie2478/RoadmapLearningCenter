using System.ComponentModel.DataAnnotations;

namespace Learning.Shared.DTOs;

/// <summary>
/// Request to execute C# code in the sandbox.
/// </summary>
public class PlaygroundExecuteRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;

    public Guid? TopicId { get; set; }

    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// Response from code execution.
/// </summary>
public class PlaygroundExecuteResponse
{
    public string Output { get; set; } = string.Empty;
    public string? Errors { get; set; }
    public int ExitCode { get; set; }
    public double ExecutionTimeMs { get; set; }
}

/// <summary>
/// Represents a saved playground session.
/// </summary>
public class PlaygroundSessionDto
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public string Language { get; set; } = "csharp";
    public string Code { get; set; } = string.Empty;
    public string? Output { get; set; }
    public string? Errors { get; set; }
    public DateTime SavedAt { get; set; }
}

/// <summary>
/// Request to save a playground session.
/// </summary>
public class PlaygroundSaveRequest
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
