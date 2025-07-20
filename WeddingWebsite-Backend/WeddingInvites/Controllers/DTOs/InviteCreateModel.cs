namespace WeddingInvites.Controllers.DTOs;

public class InviteCreateModel
{
    public string Name { get; set; }
    public List<int>? GuestIds { get; set; }
}