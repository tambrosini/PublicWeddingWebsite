using Microsoft.EntityFrameworkCore;
using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Database;
using WeddingInvites.Domain;
using WeddingInvites.Domain.FileModels;

namespace WeddingInvites.Services;

public class GuestService
{
    private readonly ILogger<GuestService> _logger;
    private readonly ApplicationDbContext _context;

    public GuestService(ILogger<GuestService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<List<Guest>> GetAllAsync()
    {
        return await _context.Guests
            .ToListAsync();
    }

    public async Task<List<Guest>> GetAvailableGuestsAsync()
    {
        return await _context.Guests
            .Where(x => x.InviteId == null)
            .ToListAsync();
    }

    public async Task<Guest?> GetAsync(int id)
    {
        return await _context.Guests
            .Include(g => g.Invite)
            .SingleOrDefaultAsync(g => g.Id == id);
    }

    public async Task<List<Guest>> GetAsync(List<int> guestIds)
    {
        return await _context.Guests
            .Include(g => g.Invite)
            .Where(g => guestIds.Contains(g.Id))
            .ToListAsync();
    }

    public async Task<DashboardModel> GetDashboardAsync()
    {
        var allGuests = await GetAllAsync();

        var dashboardModel = new DashboardModel()
        {
            TotalGuests = allGuests.Count(),
            AcceptedGuests = allGuests.Count(x => x.Attending == true),
            DeclinedGuests = allGuests.Count(x => x.Attending == false),
            PendingGuests = allGuests.Count(x => x.Attending == null),
        };

        return dashboardModel;
    }

    public async Task<IEnumerable<GuestFileModel>> GetFileModelAsync()
    {
        var models = _context.Guests
            .Select(g => new GuestFileModel
            {
                FirstName = g.FirstName,
                LastName = g.LastName,
                Attendance = g.Attending.HasValue ? (g.Attending.Value ? "Yes" : "No") : "Pending",
                DietaryRequirements = g.DietaryRequirements ?? string.Empty
            })
            .OrderBy(g => g.LastName)
            .ThenBy(g => g.FirstName);
        return await models.ToListAsync();
    }

    public async Task CreateAsync(Guest guest)
    {
        guest.Attending = null;

        await _context.Guests.AddAsync(guest);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guest guest)
    {
        _context.Guests.Remove(guest);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guest guest)
    {
        _context.Guests.Update(guest);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Guests.AnyAsync(g => g.Id == id);
    }
}