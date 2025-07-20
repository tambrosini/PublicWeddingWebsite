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
[Route("api/guest")]
public class GuestController : ControllerBase
{
    private readonly ILogger<GuestController> _logger;
    private readonly GuestService _guestService;
    private readonly InviteService _inviteService;

    public GuestController(
        ILogger<GuestController> logger,
        GuestService guestService,
        InviteService inviteService)
    {
        _logger = logger;
        _guestService = guestService;
        _inviteService = inviteService;
    }

    /// <summary>
    /// List all guests
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Guest>>> ListGuests()
    {
        var guests = await _guestService.GetAllAsync();
        return Ok(guests);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<Guest>>> GetAvailableGuests()
    {
        var guests = await _guestService.GetAvailableGuestsAsync();
        return Ok(guests);
    }
    
    [HttpGet("dashboard")]
    public async Task<ActionResult<IEnumerable<DashboardModel>>> GetDashboard()
    {
        var model = await _guestService.GetDashboardAsync();
        return Ok(model);
    }

    /// <summary>
    /// Get a single guest
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Guest>> GetGuest(int id)
    {
        var guest = await _guestService.GetAsync(id);

        if (guest == null)
            return NotFound();

        return Ok(guest);
    }    
    
    /// <summary>
    /// Create a guest, defaulting their attendance to null (pending)
    /// </summary>
    /// <param name="guest"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Guest>> CreateGuest(Guest guest)
    {
        //Default attending to false
        guest.Attending = null;
        
        await _guestService.CreateAsync(guest);

        return CreatedAtAction(nameof(GetGuest), new { id = guest.Id }, guest);
    }

    /// <summary>
    /// Delete a guest
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGuest(int id)
    {
        var guest = await _guestService.GetAsync(id);
        if (guest == null)
            return NotFound();

        await _guestService.DeleteAsync(guest);

        return NoContent();
    }
    
    /// <summary>
    /// Update a guest, ensuring they have valid data
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedGuest"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGuest(int id, Guest updatedGuest)
    {
        if (id != updatedGuest.Id)
            return BadRequest("Guest ID mismatch.");

        if(! await _guestService.ExistsAsync(id))
            return NotFound();

        // Ensure the  exists
        if (updatedGuest.InviteId.HasValue && !await _inviteService.ExistsAsync(updatedGuest.InviteId.Value))
            return BadRequest("The specified invite does not exist.");

        // Do the update
        await _guestService.UpdateAsync(updatedGuest);

        return NoContent();
    }
}