using Learning.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Learning.Server.Controllers;

/// <summary>
/// Code playground execution and session management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlaygroundController : ControllerBase
{
    private readonly Services.CodeExecutionService _exec;
    private readonly Data.AppDbContext _db;
    private readonly ILogger<PlaygroundController> _logger;

    public PlaygroundController(Services.CodeExecutionService exec, Data.AppDbContext db, ILogger<PlaygroundController> logger)
    {
        _exec = exec;
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Executes C# code in a sandboxed environment.
    /// </summary>
    [HttpPost("execute")]
    public async Task<ActionResult<PlaygroundExecuteResponse>> Execute([FromBody] PlaygroundExecuteRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return BadRequest("Code is required.");

        var result = await _exec.ExecuteAsync(request.Code, request.TopicId, request.UserId, ct);

        // Save session
        var session = new Shared.Domain.PlaygroundSession
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            TopicId = request.TopicId ?? Guid.Empty,
            Language = "csharp",
            Code = request.Code,
            Output = result.Output,
            Errors = result.Errors,
            SavedAt = DateTime.UtcNow
        };

        _db.PlaygroundSessions.Add(session);
        await _db.SaveChangesAsync(ct);

        return Ok(result);
    }

    /// <summary>
    /// Gets a saved playground session.
    /// </summary>
    [HttpGet("{sessionId}")]
    public async Task<ActionResult<PlaygroundSessionDto>> GetSession(Guid sessionId, CancellationToken ct)
    {
        var session = await _db.PlaygroundSessions.FindAsync(new object[] { sessionId }, ct);
        if (session == null) return NotFound();

        return Ok(new PlaygroundSessionDto
        {
            Id = session.Id,
            TopicId = session.TopicId,
            Language = session.Language,
            Code = session.Code,
            Output = session.Output,
            Errors = session.Errors,
            SavedAt = session.SavedAt
        });
    }

    /// <summary>
    /// Saves a playground session without executing.
    /// </summary>
    [HttpPost("save")]
    public async Task<ActionResult<PlaygroundSessionDto>> Save([FromBody] PlaygroundSaveRequest request, CancellationToken ct)
    {
        var session = new Shared.Domain.PlaygroundSession
        {
            Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id,
            UserId = request.UserId,
            TopicId = request.TopicId,
            Language = "csharp",
            Code = request.Code,
            SavedAt = DateTime.UtcNow
        };

        if (request.Id != Guid.Empty)
        {
            _db.PlaygroundSessions.Update(session);
        }
        else
        {
            _db.PlaygroundSessions.Add(session);
        }

        await _db.SaveChangesAsync(ct);

        return Ok(new PlaygroundSessionDto
        {
            Id = session.Id,
            TopicId = session.TopicId,
            Language = session.Language,
            Code = session.Code,
            SavedAt = session.SavedAt
        });
    }
}
