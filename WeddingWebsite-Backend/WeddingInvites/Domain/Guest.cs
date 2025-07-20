using System.ComponentModel.DataAnnotations;

namespace WeddingInvites.Domain;

public class Guest : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }
    
    [MaxLength(250)]
    public string? DietaryRequirements { get; set; }

    /// <summary>
    /// Tri state. null -> pending, false -> declined, true -> attending
    /// </summary>
    public bool? Attending { get; set; }

    public int? InviteId { get; set; }

    public Invite? Invite { get; set; } = null;
}