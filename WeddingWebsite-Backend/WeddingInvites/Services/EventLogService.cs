using Microsoft.EntityFrameworkCore;
using WeddingInvites.Database;
using WeddingInvites.Domain;

namespace WeddingInvites.Services;

public class EventLogService
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<EventLog> _logs;

    public EventLogService(ApplicationDbContext context)
    {
        _context = context;
        _logs = context.EventLogs;
    }

    public async Task WriteAsync(string text)
    {
        _logs.Add(new EventLog(text));
        
        await _context.SaveChangesAsync();
    }

    public Task WriteRsvpLogAsync(Invite invite, List<Guest> guests)
    {
        //Write invite log
        _logs.Add(new EventLog($"Invite for {invite.Name} has been responded to."));
        
        //Write Guest logs
        foreach (var guest in guests)
        {
            var attendanceString = guest.Attending!.Value ? "Yes" : "No";
            _logs.Add(new EventLog($"{guest.FirstName} {guest.LastName} - {attendanceString}"));
        }

        return Task.CompletedTask;
    }

    public async Task<List<EventLog>> GetLogsAsync()
    {
        return await _logs.OrderByDescending(x => x.Time).ToListAsync();
    }
}