using Learning.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Learning.Server.Controllers;

/// <summary>
/// AI proxy controller — delegates to AIService so OpenAI keys never reach the client.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly Services.AIService _ai;
    private readonly ILogger<AIController> _logger;

    public AIController(Services.AIService ai, ILogger<AIController> logger)
    {
        _ai = ai;
        _logger = logger;
    }

    /// <summary>
    /// Asks a context-threaded question about a topic block.
    /// </summary>
    [HttpPost("ask")]
    public async Task<ActionResult<AIAskResponse>> Ask([FromBody] AIAskRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest("Question is required.");

        try
        {
            var paragraph = await _ai.AnswerQuestionAsync(request, null, ct);
            return Ok(new AIAskResponse { Paragraph = new ParagraphDto
            {
                Id = paragraph.Id,
                BlockId = paragraph.BlockId,
                ParentParagraphId = paragraph.ParentParagraphId,
                Text = paragraph.Text,
                CreatedBy = paragraph.CreatedBy,
                Depth = paragraph.Depth,
                CreatedAt = paragraph.CreatedAt,
                Replies = Array.Empty<ParagraphDto>()
            }});
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Generates block content on-demand.
    /// </summary>
    [HttpPost("generate-block")]
    public async Task<ActionResult<AIGenerateBlockResponse>> GenerateBlock([FromBody] AIGenerateBlockRequest request, CancellationToken ct)
    {
        try
        {
            var content = await _ai.GenerateBlockContentAsync(request.TopicId, request.BlockId, ct);
            return Ok(new AIGenerateBlockResponse
            {
                Block = new TopicBlockDto
                {
                    Id = request.BlockId,
                    TopicId = request.TopicId,
                    Content = content,
                    Paragraphs = Array.Empty<ParagraphDto>()
                }
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
