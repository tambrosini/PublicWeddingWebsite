namespace WeddingInvites.Controllers.DTOs;

public class UpdateInviteDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    /// <summary>
    /// List of guest IDs to link to the invite
    /// </summary>
    public List<int>? GuestIds { get; set; }

    /// <summary>
    /// Only populated if manually RSVPing for guest
    /// </summary>
    public IEnumerable<GuestRsvp>? GuestRsvps { get; set; }
}
