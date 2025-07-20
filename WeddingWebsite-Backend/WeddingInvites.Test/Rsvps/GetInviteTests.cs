using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Domain;

namespace WeddingInvites.Test.Rsvps;

/// <summary>
/// Note, we are using the invite fixture as we dont have a specific RSVP entity
/// </summary>
[Collection("Tests")]
public class GetInviteTests: IClassFixture<BaseFixture<Invite>>,
    IClassFixture<BaseFixture<Guest>>
{
    private readonly BaseFixture<Invite> _fixture;
    
    private readonly BaseFixture<Guest> _guestFixture;
    
    private Invite ToRsvp { get; set; }
    
    private Invite Result { get; set; }
    
    private Guest Guest1 { get; set; }
    
    private Guest Guest2 { get; set; }

    public GetInviteTests(BaseFixture<Invite> fixture, BaseFixture<Guest> guestFixture)
    {
        _fixture = fixture;
        _fixture.Endpoint = "api/rsvp";
        
        _guestFixture = guestFixture;
        
        _fixture.SetAuthCookie();
        _guestFixture.SetAuthCookie();
        
        _fixture.Setup = async () =>
        {
            ToRsvp = new Invite()
            {
                Name = "To Rsvp",
                PublicCode = "1234Update",
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
            var requestModel = new GetInviteRequest()
            {
                InviteUniqueCode = ToRsvp.PublicCode
            };

            Result = await _fixture.PostAsync<GetInviteRequest, Invite>( requestModel, "get-invite" );
        };
    }
    
    [Fact]
    public async Task GIVEN_correct_details_THEN_should_return_valid_Invite()
    {
        await _fixture.Setup();
        
        await _fixture.Execute(null);

        Assert.NotNull(Result);
        
        Assert.Equal(ToRsvp.Id, Result.Id);
        Assert.Equal(ToRsvp.Name, Result.Name);
        Assert.Equal(2, Result.Guests.Count());
        
        var guest1 = Result.Guests.Single(x => x.Id == Guest1.Id);
        
        Assert.Equal(Guest1.FirstName, guest1.FirstName);
        Assert.Equal(Guest1.LastName, guest1.LastName);
        Assert.Equal(Guest1.Attending, guest1.Attending);
        
        var guest2 = Result.Guests.Single(x => x.Id == Guest2.Id);
        
        Assert.Equal(Guest2.FirstName, guest2.FirstName);
        Assert.Equal(Guest2.LastName, guest2.LastName);
        Assert.Equal(Guest2.Attending, guest2.Attending);
        
        await _fixture.Cleanup();
    }
}