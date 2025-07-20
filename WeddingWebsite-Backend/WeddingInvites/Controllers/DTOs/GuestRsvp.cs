using System.ComponentModel.DataAnnotations;

namespace WeddingInvites.Controllers.DTOs;

public class GuestRsvp
{
    [Required]
    public int GuestId { get; set; }
    
    [MaxLength(250)]
    public string? DietaryRequirements { get; set; }
      
    [Required]
    public bool Attending { get; set; }
}