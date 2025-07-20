using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Domain;

namespace WeddingInvites.Services;

public interface IEmailService
{
    public Task SendRsvpNotificationAsync(Invite invite, List<Guest> guests);
}

public class EmailService : IEmailService
{
    private readonly ILogger<IEmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<IEmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendRsvpNotificationAsync(Invite invite, List<Guest> guests)
    {
        var apiKey = _configuration["Email:ApiKey"];
        var apiSecret = _configuration["Email:ApiSecret"];
        var recipientEmail = _configuration["Email:To"];
        
        var message = BuildEmailMessage(invite, guests);

        MailjetClient client = new MailjetClient(apiKey, apiSecret);

        MailjetRequest request = new MailjetRequest()
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, "notifications@your.wedding")
            .Property(Send.FromName, "RSVP Notifications")
            .Property(Send.Subject, "New RSVP Details")
            .Property(Send.TextPart, message)
            .Property(Send.Recipients, new JArray
            {
                new JObject
                {
                    { "Email", recipientEmail }
                }
            });
        
        MailjetResponse response = await client.PostAsync(request);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email sent successfully");
        }
        else
        {
            _logger.LogError($"Email send failed. Status Code:{response.StatusCode}.\n" +
                             $"Error Info: {response.GetErrorInfo()}\n" +
                             $"Error Message: {response.GetErrorMessage()}");
        }
    }

    private string BuildEmailMessage(Invite invite, List<Guest> guests)
    {
        var message = $"An RSVP has been received for {invite.Name}\n";

        foreach (var g in guests)
        {
            message += $"{g.FirstName} {g.LastName} - Attending: {g.Attending!.Value} - Dietary Requirements: {g.DietaryRequirements}\n";
        }
        
        return message;
    }
}