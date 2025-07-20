using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Domain;
using WeddingInvites.Services;

namespace WeddingInvites.Controllers;

[ApiController]
[Authorize]
[Route("api/logs")]
public class EventLogController : ControllerBase
{
    private readonly EventLogService _eventLogService;

    public EventLogController(
        EventLogService eventLogService)
    {
        _eventLogService = eventLogService;
    }

    /// <summary>
    /// Get all the logs, but make them nice
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventLogModel>>> GetLogs()
    {
        var logs = await _eventLogService.GetLogsAsync();

        var models = new List<EventLogModel>();
        
        foreach (var log in logs)
        {
            models.Add( new EventLogModel()
            {
                Text = log.Text,
                Time = log.Time.ToLocalTime().ToString("G")
            });
        }
        
        return Ok(models);
    }
}