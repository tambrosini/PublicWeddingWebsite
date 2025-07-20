namespace WeddingInvites.Controllers.DTOs;

public class DashboardModel
{
    public int TotalGuests { get; set; }
    public int AcceptedGuests { get; set; }
    public int DeclinedGuests { get; set; }
    public int PendingGuests { get; set; }
}