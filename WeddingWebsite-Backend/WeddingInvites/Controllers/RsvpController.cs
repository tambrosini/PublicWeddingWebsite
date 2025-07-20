using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Domain;
using WeddingInvites.Services;

namespace WeddingInvites.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/rsvp")]
public class RsvpController : ControllerBase
{
    private readonly ILogger<RsvpController> _logger;
    private readonly RsvpService _rsvpService;
    private readonly EventLogService _eventLogService;
    private readonly FileService _fileservice;

    public RsvpController(
        ILogger<RsvpController> logger,
        RsvpService rsvpService,
        EventLogService eventLogService,
        FileService fileservice)
    {
        _logger = logger;
        _rsvpService = rsvpService;
        _eventLogService = eventLogService;
        _fileservice = fileservice;
    }

    /// <summary>
    /// Gets the invite based on the unique invite code. Returns it with guests attached
    /// </summary>
    /// <returns></returns>
    [HttpPost("get-invite")]
    public async Task<ActionResult<Invite>> GetInviteForRsvp(GetInviteRequest request)
    {
        var invite = await _rsvpService.GetInviteForRsvp(request.InviteUniqueCode);

        if (invite == null)
            return NotFound();

        await _eventLogService.WriteAsync($"Rsvp requested for {invite.Name}");

        // Check if RSVP has already been completed
        if (invite.RsvpCompleted)
        {
            return Conflict(new
            {
                errorCode = "RSVP_ALREADY_COMPLETED",
                message = "This invitation has already been responded to. If you need to make changes, please contact us directly."
            });
        }

        return Ok(invite);
    }

    /// <summary>
    /// Updates the attendance and dietaries for the guests listed
    /// </summary>
    /// <param name="rsvp"></param>
    /// <returns></returns>
    [HttpPost("update-invite")]
    public async Task<IActionResult> GuestRsvpToInvite(RsvpToInviteRequest rsvp)
    {
        if (string.IsNullOrEmpty(rsvp.InviteUniqueCode))
            return BadRequest("Invite code is required");

        var exists = await _rsvpService.InviteExistsForRsvp(rsvp.InviteUniqueCode);

        //Verify the invite exists
        if (!exists)
            return Unauthorized();
        // Check if RSVP has already been completed
        var rsvpAlreadyCompleted = await _rsvpService.IsRsvpAlreadyCompleted(rsvp.InviteUniqueCode);
        if (rsvpAlreadyCompleted)
        {
            return Conflict(new
            {
                errorCode = "RSVP_ALREADY_COMPLETED",
                message = "This invitation has already been responded to. If you need to make changes, please contact us directly."
            });
        }

        await _rsvpService.UpdateInviteForRsvp(rsvp);

        return Ok();
    }

    /// <summary>
    /// Wake up endpoint to keep the Azure database connection active
    /// </summary>
    /// <returns></returns>
    [HttpGet("wake-up")]
    public async Task<IActionResult> WakeUp()
    {
        await _rsvpService.WakeUpDatabase();
        return Ok();
    }

    /// <summary>
    /// Gets a spreadsheet of all the guests, their attendance and dietary requirements
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("download")]
    public async Task<IActionResult> DownloadRsvpReport()
    {
        var wb = await _fileservice.ExportGuestList();

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"GuestList_{DateTime.Now:dd-MM-yyyy}.xlsx";

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

}