using System.Diagnostics.CodeAnalysis;
using Learning.Shared.DTOs;

namespace Learning.Client.Services;

/// <summary>
/// Simple in-memory UI state for the current session.
/// Tracks expanded blocks, paragraph threads, current topic, and playground code.
/// Registered as Scoped DI in Program.cs — one instance per page lifecycle.
/// </summary>
public class StateService
{
    /// <summary>Currently viewed topic, or Guid.Empty if none selected.</summary>
    public Guid CurrentTopicId { get; private set; }

    /// <summary>Block IDs that are expanded in the topic detail view.</summary>
    public HashSet<Guid> ExpandedBlocks { get; } = [];

    /// <summary>Paragraph threads grouped by block ID.</summary>
    public Dictionary<Guid, List<ParagraphDto>> ParagraphThreads { get; } = [];

    /// <summary>Current playground editor content.</summary>
    public string PlaygroundCode { get; private set; } = string.Empty;

    /// <summary>Expands or collapses a block. Idempotent — toggling an expanded block removes it.</summary>
    public void ToggleBlock(Guid blockId)
    {
        if (!ExpandedBlocks.Remove(blockId))
        {
            ExpandedBlocks.Add(blockId);
        }
    }

    /// <summary>Manually set a block as expanded.</summary>
    public void ExpandBlock(Guid blockId) => ExpandedBlocks.Add(blockId);

    /// <summary>Manually collapse a block.</summary>
    public void CollapseBlock(Guid blockId) => ExpandedBlocks.Remove(blockId);

    /// <summary>Get the paragraph threads for a block, returning an empty list if none exist.</summary>
    public List<ParagraphDto> GetExpandedParagraphs(Guid blockId)
    {
        if (ParagraphThreads.TryGetValue(blockId, out var threads))
        {
            return threads;
        }
        return [];
    }

    /// <summary>Set the paragraph threads for a given block, replacing any existing threads.</summary>
    public void SetParagraphs(Guid blockId, List<ParagraphDto> paragraphs)
    {
        ParagraphThreads[blockId] = paragraphs;
    }

    /// <summary>Add a paragraph to the threads for a given block.</summary>
    public void AddParagraph(Guid blockId, ParagraphDto paragraph)
    {
        if (!ParagraphThreads.TryGetValue(blockId, out var threads))
        {
            threads = [];
            ParagraphThreads[blockId] = threads;
        }
        threads.Add(paragraph);
    }

    /// <summary>Set the currently active topic.</summary>
    public void SetCurrentTopic(Guid topicId)
    {
        if (CurrentTopicId == topicId) return;
        CurrentTopicId = topicId;
    }

    /// <summary>Set the playground editor code.</summary>
    public void SetPlaygroundCode(string code)
    {
        PlaygroundCode = code ?? string.Empty;
    }

    /// <summary>Reset all state — clears expanded blocks, paragraph threads, and topic context.</summary>
    public void Reset()
    {
        CurrentTopicId = Guid.Empty;
        ExpandedBlocks.Clear();
        ParagraphThreads.Clear();
        PlaygroundCode = string.Empty;
    }
}
