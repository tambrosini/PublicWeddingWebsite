namespace WeddingInvites.Controllers.DTOs;

public class RsvpToInviteRequest
{
    public int InviteId { get; set; }
    
    public string? InviteUniqueCode { get; set; }
       
    public IEnumerable<GuestRsvp> GuestRsvps { get; set; }
}