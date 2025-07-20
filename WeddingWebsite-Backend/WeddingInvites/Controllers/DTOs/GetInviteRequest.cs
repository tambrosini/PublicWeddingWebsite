namespace WeddingInvites.Controllers.DTOs;

public class GetInviteRequest
{
    /// <summary>
    /// Maps to the PublicCode on the Invite model
    /// </summary>
    public string InviteUniqueCode { get; set; }
}