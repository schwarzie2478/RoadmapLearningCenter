using Learning.Server.Data;
using Learning.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learning.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoadmapController : ControllerBase
{
    private readonly AppDbContext _db;

    public RoadmapController(AppDbContext db) => _db = db;

    /// <summary>
    /// Returns the full roadmap tree.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<RoadmapResponse>> GetAll(CancellationToken ct)
    {
        var topics = await _db.RoadmapTopics
            .Include(t => t.Children)
            .ToListAsync(ct);

        var topicDtos = topics
            .Where(t => t.ParentId == null)
            .Select(t => ToTreeDto(t, topics))
            .ToList();

        return Ok(new RoadmapResponse { Topics = topicDtos });
    }

    /// <summary>
    /// Returns a single track's roadmap.
    /// </summary>
    [HttpGet("{track}")]
    public async Task<ActionResult<RoadmapResponse>> GetByTrack(string track, CancellationToken ct)
    {
        if (!Enum.TryParse<Shared.Enums.RoadmapTrack>(track, ignoreCase: true, out var trackEnum))
            return BadRequest($"Unknown track: {track}");

        var topics = await _db.RoadmapTopics
            .Where(t => t.Track == trackEnum)
            .Include(t => t.Children)
            .ToListAsync(ct);

        var topicDtos = topics
            .Where(t => t.ParentId == null)
            .Select(t => ToTreeDto(t, topics))
            .ToList();

        return Ok(new RoadmapResponse { Topics = topicDtos });
    }

    private static RoadmapTopicDto ToTreeDto(Shared.Domain.RoadmapTopic topic, List<Shared.Domain.RoadmapTopic> all)
    {
        return new RoadmapTopicDto
        {
            Id = topic.Id,
            ParentId = topic.ParentId,
            Title = topic.Title,
            Slug = topic.Slug,
            Description = topic.Description,
            Track = topic.Track,
            Order = topic.Order,
            Difficulty = topic.Difficulty,
            Prerequisites = topic.Prerequisites.ToList(),
            Children = all
                .Where(c => c.ParentId == topic.Id)
                .Select(c => ToTreeDto(c, all))
                .OrderBy(c => c.Order)
                .ToList()
        };
    }
}
