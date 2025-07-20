using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Domain;

namespace WeddingInvites.Test.Rsvps;

[Collection("Tests")]
public class RsvpToInviteTests : IClassFixture<BaseFixture<Invite>>,
    IClassFixture<BaseFixture<Guest>>
{
    private readonly BaseFixture<Invite> _fixture;
    
    private readonly BaseFixture<Guest> _guestFixture;
    
    private Invite ToRsvp { get; set; }
    
    private bool Result { get; set; }
    
    private Invite RetrievedInvite { get; set; }
    
    private Guest Guest1 { get; set; }
    
    private Guest Guest2 { get; set; }

    private string Dietary1 = "Gluten free";
    
    private string Dietary2 = "Vegan";

    public RsvpToInviteTests(BaseFixture<Invite> fixture, BaseFixture<Guest> guestFixture)
    {
        _fixture = fixture;
        _fixture.Endpoint = "api/rsvp";
        
        _guestFixture = guestFixture;
        
        _fixture.SetAuthCookie();
        _guestFixture.SetAuthCookie();
        
        _fixture.Setup = async () =>
        {
            // Ensure database contexts are fresh
            await _fixture.TestDataManager.GenerateNewDbContext();
            await _guestFixture.TestDataManager.GenerateNewDbContext();
            
            ToRsvp = new Invite()
            {
                Name = "To Rsvp",
                PublicCode = "123Update",
                RsvpCompleted = false,
            };
            
            await _fixture.TearUpAsync(ToRsvp);
            
            Guest1 = new Guest()
            {
                FirstName = "James",
                LastName = "Holt",
                Attending = null,
                InviteId = ToRsvp.Id
            };
            
            Guest2 = new Guest()
            {
                FirstName = "Meg",
                LastName = "Holt",
                Attending = null,
                InviteId = ToRsvp.Id
            };
            
            await _guestFixture.TearUpAsync([Guest1, Guest2]);
        };

        _fixture.Execute = async (_) =>
        {
            var guest1Rsvp = new GuestRsvp()
            {
                GuestId = Guest1.Id,
                Attending = true,
                DietaryRequirements = Dietary1
            };

            var guest2Rsvp = new GuestRsvp()
            {
                GuestId = Guest2.Id,
                Attending = true,
                DietaryRequirements = Dietary2
            };

            var inviteRsvp = new RsvpToInviteRequest()
            {
                InviteId = ToRsvp.Id,
                GuestRsvps = [guest1Rsvp, guest2Rsvp],
                InviteUniqueCode = ToRsvp.PublicCode
            };

            Result = await _fixture.PostAsync<RsvpToInviteRequest>(inviteRsvp, "update-invite");

            // Create a fresh context before getting the invite
            await _fixture.TestDataManager.GenerateNewDbContext();
            
            _fixture.Endpoint = "api/invite";

            RetrievedInvite = await _fixture.GetByIdAsync(ToRsvp.Id);
        };
        
        // Ensure proper cleanup
        _fixture.Cleanup = async () =>
        {
            await _fixture.TestDataManager.GenerateNewDbContext();
            await _guestFixture.TestDataManager.GenerateNewDbContext();
            
            await _fixture.TestDataManager.Cleanup();
            await _guestFixture.TestDataManager.Cleanup();
        };
    }
    
    
    [Fact]
    public async Task GIVEN_Guest_Rsvp_THEN_Return_Ok_AND_Invite_should_be_valid()
    {
        // Run setup with fresh context
        await _fixture.TestDataManager.GenerateNewDbContext();
        await _guestFixture.TestDataManager.GenerateNewDbContext();
        await _fixture.Setup();
        
        // Execute test with fresh context
        await _fixture.Execute(null);
        
        Assert.True(Result);
        
        Assert.NotNull(RetrievedInvite);
        
        Assert.True(RetrievedInvite.RsvpCompleted);
        
        // Guest checks
        Assert.Equal(2, RetrievedInvite.Guests.Count());
        
        var guestIds = RetrievedInvite.Guests.Select(x => x.Id).ToList();

        Assert.Contains(Guest1.Id, guestIds);
        Assert.Contains(Guest2.Id, guestIds);
          // Guest 1
        var guest1 = RetrievedInvite.Guests.First(x => x.Id == Guest1.Id);
        Assert.Equal(true, guest1.Attending);
        Assert.Equal(Dietary1, guest1.DietaryRequirements);
        
        // Guest 2
        var guest2 = RetrievedInvite.Guests.First(x => x.Id == Guest2.Id);
        Assert.Equal(true, guest2.Attending);
        Assert.Equal(Dietary2, guest2.DietaryRequirements);
        
        // Run cleanup with fresh context
        await _fixture.TestDataManager.GenerateNewDbContext();
        await _guestFixture.TestDataManager.GenerateNewDbContext();
        await _fixture.Cleanup();
    }
}