using WeddingInvites.Domain;

namespace WeddingInvites.Test.Invites;

[Collection("Tests")]
public class GetTests : IClassFixture<BaseFixture<Invite>>,
    IClassFixture<BaseFixture<Guest>>
{
    private readonly BaseFixture<Invite> _fixture;
    
    private readonly BaseFixture<Guest> _guestFixture;
    
    private Invite ToGet { get; set; }
    
    private Invite ToGet2 { get; set; }
    
    private Guest Guest1 {get; set;}

    private Guest Guest2 {get; set;}
    
    private List<Invite> ListResult { get; set; }
    
    private Invite SingleResult { get; set; }

    public GetTests(BaseFixture<Invite> fixture, BaseFixture<Guest> guestFixture)
    {
        _fixture = fixture;
        _guestFixture = guestFixture;
        
        _fixture.SetAuthCookie();
        _guestFixture.SetAuthCookie();

        _fixture.Setup = async () =>
        {
            ToGet = new Invite()
            {
                Name = "Doe Smith Family",
                RsvpCompleted = true,
                PublicCode = "123456",
            };

            ToGet2 = new Invite()
            {
                Name = "Empty Invite",
                RsvpCompleted = false,
                PublicCode = "654321",
            };
            
            await _fixture.TearUpAsync([ToGet, ToGet2]);
            
            Guest1 = new Guest()
            {
                FirstName = "John",
                LastName = "Doe",
                Attending = true,
                DietaryRequirements = "Eggs",
                InviteId = ToGet.Id
            };

            Guest2 = new Guest()
            {
                FirstName = "Jane",
                LastName = "Smith",
                Attending = false,
                DietaryRequirements = "Eggs",
                InviteId = ToGet.Id
            };
            
            await _guestFixture.TearUpAsync([Guest1, Guest2]);
            
        };

        _fixture.Execute = async (_) =>
        {
            SingleResult = await _fixture.GetByIdAsync(ToGet.Id);

            ListResult = await _fixture.ListAsync<Invite>();
        };
    }


    [Fact]
    public async Task GIVEN_Invites_Exist_When_GetByIdAsync_AND_List_THEN_Results_Returned()
    {
        await _fixture.Setup();

        await _fixture.Execute(null);
        
        // Single Result assertions
        Assert.Equal(ToGet.Id, SingleResult.Id);
        Assert.Equal(ToGet.Name, SingleResult.Name);
        Assert.Equal("123456", SingleResult.PublicCode);
        Assert.True(SingleResult.RsvpCompleted);
    
        // Check guests in single result
        Assert.NotNull(SingleResult.Guests);
        Assert.Equal(2, SingleResult.Guests.Count());
    
        var singleResultGuests = SingleResult.Guests.ToList();
    
        // Guest1 assertions
        var johnGuest = singleResultGuests.FirstOrDefault(g => g.FirstName == "John");
        Assert.NotNull(johnGuest);
        Assert.Equal("Doe", johnGuest.LastName);
        Assert.True(johnGuest.Attending);
        Assert.Equal("Eggs", johnGuest.DietaryRequirements);
        Assert.Equal(ToGet.Id, johnGuest.InviteId);
    
        // Guest2 assertions
        var janeGuest = singleResultGuests.FirstOrDefault(g => g.FirstName == "Jane");
        Assert.NotNull(janeGuest);
        Assert.Equal("Smith", janeGuest.LastName);
        Assert.False(janeGuest.Attending);
        Assert.Equal("Eggs", janeGuest.DietaryRequirements);
        Assert.Equal(ToGet.Id, janeGuest.InviteId);
    
        // List Results assertions
        Assert.NotNull(ListResult);
        Assert.True(ListResult.Count >= 2);
    
        // Find both invites in the list
        var firstInvite = ListResult.FirstOrDefault(i => i.PublicCode == "123456");
        var secondInvite = ListResult.FirstOrDefault(i => i.PublicCode == "654321");
    
        // First invite assertions
        Assert.NotNull(firstInvite);
        Assert.Equal(ToGet.Id, firstInvite.Id);
        Assert.Equal(ToGet.Name, firstInvite.Name);
        Assert.True(firstInvite.RsvpCompleted);
        Assert.NotNull(firstInvite.Guests);
        Assert.Equal(2, firstInvite.Guests.Count());
    
        // Second invite assertions
        Assert.NotNull(secondInvite);
        Assert.Equal(ToGet2.Id, secondInvite.Id);
        Assert.Equal(ToGet2.Name, secondInvite.Name);
        Assert.False(secondInvite.RsvpCompleted);
        Assert.Empty(secondInvite.Guests);
        
        await _fixture.Cleanup();
    }

}