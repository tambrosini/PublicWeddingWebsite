using System.ComponentModel.DataAnnotations;

namespace WeddingInvites.Domain;

public class EventLog : BaseEntity
{
    public EventLog(string text)
    {
        Text = text;
        Time = DateTime.UtcNow;
    }
    
    /// <summary>
    /// The visually pleasing description of what happened
    /// </summary>
    [Required]
    [MaxLength(300)]
    public string Text { get; set; }
    
    /// <summary>
    /// Time in UTC
    /// </summary>
    [Required]
    public DateTime Time { get; set; }
}