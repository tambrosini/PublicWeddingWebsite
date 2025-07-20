using Microsoft.EntityFrameworkCore;
using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Database;
using WeddingInvites.Domain;

namespace WeddingInvites.Services;

public class InviteService
{
    private readonly ILogger<InviteService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly GuestService _guestService;

    public InviteService(ILogger<InviteService> logger, ApplicationDbContext context, GuestService guestService)
    {
        _logger = logger;
        _context = context;
        _guestService = guestService;
    }
    
    // Helper method to generate a 10-character alphanumeric code
    private static string GeneratePublicCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Invites.AnyAsync(g => g.Id == id);
    }
    
    public async Task<List<Invite>> GetAllAsync()
    {
        return await _context.Invites
            .Include(i => i.Guests)
            .ToListAsync();
    }

    public async Task<Invite?> GetAsync(int id)
    {
        return await _context.Invites
            .Include(i => i.Guests)
            .SingleOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Invite> CreateAsync(InviteCreateModel inviteDto)
    {
        var invite = new Invite()
        {
            Name = inviteDto.Name,
            PublicCode = GeneratePublicCode(),
            RsvpCompleted = false,
        };
        
        await _context.Invites.AddAsync(invite);

        await _context.SaveChangesAsync();
        
        // Validate and update guests
        if (inviteDto.GuestIds != null && inviteDto.GuestIds.Any())
        {
            var guests = await _guestService.GetAsync(inviteDto.GuestIds);

            //TODO: Error handling filter
            // if (guests.Count != inviteDto.GuestIds.Count)
            //     return BadRequest("One or more Guest IDs are invalid.");

            // Link guests to the invite and update them
            foreach (var guest in guests)
            {
                guest.InviteId = invite.Id;
                // May need to do a raw context update as there is a saved changes in this method
                await _guestService.UpdateAsync(guest);
            }
        }

        return invite;
    }
    
    public async Task DeleteAsync(Invite invite)
    {
        _context.Invites.Remove(invite);
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateInviteDto inviteDto, Invite invite)
    {
        // Validate and update guests
        if (inviteDto.GuestIds != null && inviteDto.GuestIds.Any())
        {
            var guests = await _guestService.GetAsync(inviteDto.GuestIds);

            //TODO: Error handling filter
            // if (guests.Count != inviteDto.GuestIds.Count)
            //     return BadRequest("One or more Guest IDs are invalid.");

            // Link guests to the invite and update them
            foreach (var guest in guests)
            {
                guest.InviteId = invite.Id;
                // May need to do a raw context update as there is a saved changes in this method
                await _guestService.UpdateAsync(guest);
            }
        }
        
        invite.Name = inviteDto.Name;
        
        _context.Invites.Update(invite);
        
        await _context.SaveChangesAsync();
    }
}