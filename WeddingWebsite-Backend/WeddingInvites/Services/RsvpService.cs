using Microsoft.EntityFrameworkCore;
using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Database;
using WeddingInvites.Domain;

namespace WeddingInvites.Services;

public class RsvpService
{
    private readonly ILogger<RsvpService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly EventLogService _eventLogService;

    public RsvpService(
        ILogger<RsvpService> logger, 
        ApplicationDbContext context,
        IEmailService emailService,
        EventLogService eventLogService)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
        _eventLogService = eventLogService;
    }

    public async Task<Invite?> GetInviteForRsvp(string inviteUniqueCode)
    {
        return await _context.Invites
            .Where(i => i.PublicCode.ToLower().Equals(inviteUniqueCode.ToLower()))
            .Include(x => x.Guests).FirstOrDefaultAsync();
    }
      public async Task<bool> InviteExistsForRsvp(string inviteUniqueCode)
    {
        return await _context.Invites
            .AnyAsync(i => i.PublicCode.ToLower().Equals(inviteUniqueCode.ToLower()));
    }

    public async Task<bool> IsRsvpAlreadyCompleted(string inviteUniqueCode)
    {
        return await _context.Invites
            .Where(i => i.PublicCode.ToLower().Equals(inviteUniqueCode.ToLower()))
            .Select(i => i.RsvpCompleted)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateInviteForRsvp(RsvpToInviteRequest rsvp)
    {
        var guestIds = rsvp.GuestRsvps.Select(g => g.GuestId).ToList();
        var guests = await _context.Guests.Where(g => guestIds.Contains(g.Id)).ToListAsync();

        foreach (var guest in guests)
        {
            var guestDetails = rsvp.GuestRsvps.First(g => g.GuestId == guest.Id);
            guest.Attending = guestDetails.Attending;
            guest.DietaryRequirements = guestDetails.DietaryRequirements;
        }
        
        _context.Guests.UpdateRange(guests);
        
        var invite = await _context.Invites.SingleAsync(x => x.Id == rsvp.InviteId);
        
        invite.RsvpCompleted = true;
        
        _context.Invites.Update(invite);
        
        await _eventLogService.WriteRsvpLogAsync(invite, guests);

        await _context.SaveChangesAsync();
        
        //TODO: Keep an eye on this. We may need to force the system not to wait
        await _emailService.SendRsvpNotificationAsync(invite, guests);
    }

    public async Task WakeUpDatabase()
    {
        // Simple count operation to wake up the database
        await _context.Invites.CountAsync();
    }
}