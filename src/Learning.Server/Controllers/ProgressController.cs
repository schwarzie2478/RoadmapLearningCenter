using Learning.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Learning.Server.Controllers;

/// <summary>
/// Learner progress and review endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly Services.ProgressService _service;

    public ProgressController(Services.ProgressService service) => _service = service;

    /// <summary>
    /// Gets full progress overview for a user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ProgressOverviewDto>> GetOverview([FromQuery] string userId = "", CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("userId query parameter is required.");

        var overview = await _service.GetOverviewAsync(userId, ct);
        return Ok(overview);
    }

    /// <summary>
    /// Gets topics that need review.
    /// </summary>
    [HttpGet("review")]
    public async Task<ActionResult<List<LearnerTopicStateDto>>> GetReviewList([FromQuery] string userId = "", CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("userId query parameter is required.");

        var reviewList = await _service.GetReviewListAsync(userId, ct);
        return Ok(reviewList);
    }

    /// <summary>
    /// Updates progress for a specific topic.
    /// </summary>
    [HttpPatch("{topicId}")]
    public async Task<ActionResult<LearnerTopicStateDto>> UpdateProgress(Guid topicId, [FromBody] UpdateProgressRequest request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.UserId))
            return BadRequest("userId is required.");

        var result = await _service.UpdateStateAsync(request.UserId, topicId, request, ct);
        return Ok(result);
    }
}
