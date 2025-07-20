using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Database;
using WeddingInvites.Domain;
using WeddingInvites.Services;

namespace WeddingInvites.Controllers;

[ApiController]
[Authorize]
[Route("api/invite")]
public class InviteController : ControllerBase
{
    private readonly ILogger<InviteController> _logger;
    private readonly InviteService _inviteService;
    private readonly GuestService _guestService;
    private readonly RsvpService _rsvpService;

    public InviteController(
        ILogger<InviteController> logger, 
        InviteService inviteService, 
        GuestService guestService,
        RsvpService rsvpService)
    {
        _logger = logger;
        _inviteService = inviteService;
        _guestService = guestService;
        _rsvpService = rsvpService;
    }

    /// <summary>
    /// List all invites
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invite>>> GetInvites()
    {
        var invites = await _inviteService.GetAllAsync();
        return Ok(invites);
    }

    /// <summary>
    /// Get a single invite
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Invite>> GetInvite(int id)
    {
        var invite = await _inviteService.GetAsync(id);

        if (invite == null)
            return NotFound();

        return Ok(invite);
    }

    /// <summary>
    /// Create an invitation, auto generates an invitation code
    /// </summary>
    /// <param name="inviteModel"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Invite>> CreateInvite(InviteCreateModel inviteModel)
    {
        var invite = await _inviteService.CreateAsync(inviteModel);

        return CreatedAtAction(nameof(GetInvite), new { id = invite.Id }, invite);
    }

    /// <summary>
    /// Delete an invitation
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvite(int id)
    {
        var invite = await _inviteService.GetAsync(id);
        if (invite == null)
            return NotFound();

        await _inviteService.DeleteAsync(invite);

        return NoContent();
    }

    /// <summary>
    /// Updates the guests that belong to an invite
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateInviteDto updateDto)
    {
        if (id != updateDto.Id)
            return BadRequest("Invite ID mismatch.");

        var invite = await _inviteService.GetAsync(id);

        if (invite == null)
            return NotFound();
        
        await _inviteService.UpdateAsync(updateDto, invite);
        
        return Ok(invite);
    }
    
    /// <summary>
    /// RsvpTo an invitation. Rsvps to the invite manually
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("admin-rsvp/{id}")]
    public async Task<IActionResult> AdminRsvpToInvite(int id, UpdateInviteDto updateDto)
    {
        if (id != updateDto.Id)
            return BadRequest("Invite ID mismatch.");

        var invite = await _inviteService.GetAsync(id);

        if (invite == null)
            return NotFound();
        
        if (invite.RsvpCompleted)
            return BadRequest("Rsvp has already been completed for this invite. If you need to change " +
                              "your rsvp, please contact us directly.");

        // This is a guest responding, so we need to make sure they can't respond twice
        invite.RsvpCompleted = true;

        await _rsvpService.UpdateInviteForRsvp(new RsvpToInviteRequest()
        {
            InviteId = id,
            GuestRsvps = updateDto.GuestRsvps
        });
        
        return Ok(invite);
    }

}