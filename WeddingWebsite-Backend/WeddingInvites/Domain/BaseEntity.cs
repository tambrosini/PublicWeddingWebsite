using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WeddingInvites.Domain;

public class BaseEntity
{
    public BaseEntity()
    {
        RowVersion = Array.Empty<byte>();
    }
    
    [DataMember(Order = 1)]
    [Column(Order = 1)]
    [Key]
    [Required]
    public int Id { get; set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; }
}