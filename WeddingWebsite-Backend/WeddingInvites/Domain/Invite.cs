using System.ComponentModel.DataAnnotations;

namespace WeddingInvites.Domain;

public class Invite : BaseEntity
{
    public string Name { get; set; }
    
    public IEnumerable<Guest> Guests { get; set; } = new List<Guest>();
    
    [MaxLength(10)]
    public string PublicCode { get; set; }
    
    public bool RsvpCompleted { get; set; }
}