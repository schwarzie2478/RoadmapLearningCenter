using System.Text.Json;
using Learning.Server.Data;
using Learning.Server.Services;
using Learning.Shared.Domain;
using Learning.Shared.DTOs;
using Learning.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learning.Server.Controllers;

/// <summary>
/// Topic blocks, paragraphs, and on-demand AI generation endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TopicController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AIService _ai;
    private readonly ILogger<TopicController> _logger;

    public TopicController(AppDbContext db, AIService ai, ILogger<TopicController> logger)
    {
        _db = db;
        _ai = ai;
        _logger = logger;
    }

    /// <summary>
    /// Returns topic summary with block headers (no content).
    /// </summary>
    [HttpGet("{topicId}")]
    public async Task<ActionResult<TopicSummaryDto>> GetTopic(Guid topicId, [FromQuery] string userId = "", CancellationToken ct = default)
    {
        var topic = await _db.RoadmapTopics
            .Include(t => t.Blocks.OrderBy(b => b.Order))
            .FirstOrDefaultAsync(t => t.Id == topicId, ct);

        if (topic == null) return NotFound();

        var blocks = topic.Blocks.Select(b => new BlockHeaderDto
        {
            Id = b.Id,
            Type = b.Type,
            Title = b.Title,
            Order = b.Order
        }).ToList();

        LearnerTopicState? state = null;
        if (!string.IsNullOrEmpty(userId))
        {
            state = await _db.LearnerTopicStates.FirstOrDefaultAsync(s => s.UserId == userId && s.TopicId == topicId, ct);
        }

        var result = new TopicSummaryDto
        {
            Id = topic.Id,
            Title = topic.Title,
            Description = topic.Description,
            Track = topic.Track,
            Difficulty = topic.Difficulty,
            Blocks = blocks,
            LearnerStatus = state?.Status,
            LearnerConfidence = state?.Confidence
        };

        return Ok(result);
    }

    /// <summary>
    /// Expands a block — generates content via AI if not already generated, returns full block with paragraphs.
    /// </summary>
    [HttpGet("{topicId}/blocks/{blockId}")]
    public async Task<ActionResult<TopicBlockDto>> GetBlock(Guid topicId, Guid blockId, CancellationToken ct)
    {
        var block = await _db.TopicBlocks
            .Include(b => b.Paragraphs)
            .FirstOrDefaultAsync(b => b.Id == blockId && b.TopicId == topicId, ct);

        if (block == null) return NotFound();

        // Check if we already have paragraph content (i.e., already generated)
        var existingParagraphs = block.Paragraphs.Where(p => p.CreatedBy == ContentAuthor.System).ToList();

        string content;
        if (existingParagraphs.Count == 0)
        {
            // Generate and persist content on-demand.
            // Guard against concurrent requests that both see Count == 0.
            content = await _ai.GenerateBlockContentAsync(topicId, blockId, ct);

            var saved = await _db.ParagraphThreads
                .FirstOrDefaultAsync(p => p.BlockId == blockId && p.CreatedBy == ContentAuthor.System, ct);

            if (saved == null)
            {
                var systemPara = new ParagraphThread
                {
                    Id = Guid.NewGuid(),
                    BlockId = blockId,
                    Text = content,
                    CreatedBy = ContentAuthor.System,
                    Depth = 0,
                    CreatedAt = DateTime.UtcNow
                };
                _db.ParagraphThreads.Add(systemPara);
                await _db.SaveChangesAsync(ct);
                existingParagraphs = new List<ParagraphThread> { systemPara };
            }
            else
            {
                // A concurrent request already inserted the paragraph — use it.
                existingParagraphs = new List<ParagraphThread> { saved };
                content = saved.Text;
            }
        }
        else
        {
            content = existingParagraphs[0].Text;
        }

        var blockDto = new TopicBlockDto
        {
            Id = block.Id,
            TopicId = block.TopicId,
            Type = block.Type,
            Title = block.Title,
            Content = content,
            Paragraphs = existingParagraphs.Select(p => ToParagraphDto(p)).ToList(),
            StarterCode = null
        };

        return Ok(blockDto);
    }

    /// <summary>
    /// Adds a paragraph to the thread (user question or AI response).
    /// </summary>
    [HttpPost("{topicId}/blocks/{blockId}/paragraphs")]
    public async Task<ActionResult<ParagraphDto>> AddParagraph(Guid topicId, Guid blockId,
        [FromBody] AIAskRequest request, CancellationToken ct)
    {
        var block = await _db.TopicBlocks
            .Include(b => b.Topic)
            .Include(b => b.Paragraphs)
            .FirstOrDefaultAsync(b => b.Id == blockId && b.TopicId == topicId, ct);

        if (block == null) return NotFound();

        // Get parent context (if replying to a specific paragraph)
        List<ParagraphThread>? parentContext = null;
        if (request.ParagraphId.HasValue)
        {
            var parent = block.Paragraphs.FirstOrDefault(p => p.Id == request.ParagraphId.Value);
            if (parent == null)
            {
                // Could be a deeper nested reply — fetch the full thread up to parent
                parent = await _db.ParagraphThreads.FirstOrDefaultAsync(p => p.Id == request.ParagraphId.Value, ct);
            }
            if (parent != null)
            {
                // Get siblings and ancestors for context
                var ancestors = await GetAncestorsAsync(parent.Id, ct);
                parentContext = ancestors;
                parentContext.Add(parent);
            }
        }

        var answer = await _ai.AnswerQuestionAsync(request, parentContext, ct);

        // Save the paragraph
        _db.ParagraphThreads.Add(answer);
        await _db.SaveChangesAsync(ct);

        return Ok(ToParagraphDto(answer));
    }

    /// <summary>
    /// Adds a user-authored paragraph to the thread (e.g., user notes).
    /// </summary>
    [HttpPost("{topicId}/blocks/{blockId}/paragraphs/user")]
    public async Task<ActionResult<ParagraphDto>> AddUserParagraph(Guid topicId, Guid blockId,
        [FromBody] ParagraphTextDto request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Text is required.");

        var block = await _db.TopicBlocks.AnyAsync(b => b.Id == blockId && b.TopicId == topicId, ct);
        if (!block) return NotFound();

        ParagraphThread? parent = null;
        if (request.ParentParagraphId.HasValue)
            parent = await _db.ParagraphThreads.FirstOrDefaultAsync(p => p.Id == request.ParentParagraphId, ct);

        var paragraph = new ParagraphThread
        {
            Id = Guid.NewGuid(),
            BlockId = blockId,
            ParentParagraphId = request.ParentParagraphId,
            Text = request.Text,
            CreatedBy = ContentAuthor.User,
            Depth = parent?.Depth + 1 ?? 0,
            CreatedAt = DateTime.UtcNow
        };

        _db.ParagraphThreads.Add(paragraph);
        await _db.SaveChangesAsync(ct);

        return Ok(ToParagraphDto(paragraph));
    }

    private async Task<List<ParagraphThread>> GetAncestorsAsync(Guid paragraphId, CancellationToken ct)
    {
        var ancestors = new List<ParagraphThread>();
        var current = await _db.ParagraphThreads.FindAsync(new object[] { paragraphId }, ct);

        while (current?.ParentParagraphId != null)
        {
            current = await _db.ParagraphThreads.FindAsync(new object[] { current.ParentParagraphId.Value }, ct);
            if (current != null) ancestors.Insert(0, current);
        }

        return ancestors;
    }

    private static ParagraphDto ToParagraphDto(ParagraphThread p)
    {
        return new ParagraphDto
        {
            Id = p.Id,
            BlockId = p.BlockId,
            ParentParagraphId = p.ParentParagraphId,
            Text = p.Text,
            CreatedBy = p.CreatedBy,
            Depth = p.Depth,
            CreatedAt = p.CreatedAt,
            Replies = Array.Empty<ParagraphDto>() // Loaded separately if needed
        };
    }
}

/// <summary>
/// Simple DTO for posting user text.
/// </summary>
public record ParagraphTextDto(string? Text, Guid? ParentParagraphId = null);
