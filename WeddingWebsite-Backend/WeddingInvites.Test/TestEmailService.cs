using WeddingInvites.Domain;
using WeddingInvites.Services;

namespace WeddingInvites.Test;

public class TestEmailService : IEmailService
{
    public async Task SendRsvpNotificationAsync(Invite invite, List<Guest> guests)
    {
        await Task.CompletedTask;
    }
}