using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Domain;

namespace WeddingInvites.Test.Invites;

[Collection("Tests")]
public class UpdateTests : IClassFixture<BaseFixture<Invite>>,
    IClassFixture<BaseFixture<Guest>>
{
    
    private readonly BaseFixture<Invite> _fixture;
    
    private readonly BaseFixture<Guest> _guestFixture;
    
    private Invite ToUpdate { get; set; }
    
    private Invite Result { get; set; }
    
    private Guest Guest1 { get; set; }
    
    private Guest Guest2 { get; set; }

    private string NewName = "Some new name";

    public UpdateTests(BaseFixture<Invite> fixture, BaseFixture<Guest> guestFixture)
    {
        _fixture = fixture;
        _guestFixture = guestFixture;
        
        _fixture.SetAuthCookie();
        _guestFixture.SetAuthCookie();

        _fixture.Setup = async () =>
        {
            ToUpdate = new Invite()
            {
                Name = "To Update",
                PublicCode = "123Update",
                RsvpCompleted = false,
            };
            
            await _fixture.TearUpAsync(ToUpdate);
            
            Guest1 = new Guest()
            {
                FirstName = "James",
                LastName = "Holt",
                Attending = null,
                InviteId = ToUpdate.Id
            };
            
            // We'll be adding this guest
            Guest2 = new Guest()
            {
                FirstName = "Meg",
                LastName = "Holt",
                Attending = null,
                InviteId = null
            };
            
            await _guestFixture.TearUpAsync([Guest1, Guest2]);
        };

        _fixture.Execute = async (_) =>
        {
            var updatedEntity = new UpdateInviteDto()
            {
                Id = ToUpdate.Id,
                GuestIds = [Guest1.Id, Guest2.Id],
                GuestRsvps = null,
                Name = NewName
            };
            
            Result = await _fixture.UpdateAsync<UpdateInviteDto, Invite>(updatedEntity, ToUpdate.Id);
        };
    }
    
    [Fact]
    public async Task GIVEN_Update_Invite_THEN_Result_values_should_match()
    {
        await _fixture.Setup();
        
        await _fixture.Execute(null);
        
        Assert.NotNull(Result);
        Assert.Equal(ToUpdate.Id, Result.Id);
        // Name Changed
        Assert.NotEqual(ToUpdate.Name, Result.Name);
        Assert.Equal(NewName, Result.Name);
        Assert.Equal(ToUpdate.PublicCode, Result.PublicCode);
        Assert.Equal(ToUpdate.RsvpCompleted, Result.RsvpCompleted);
        
        //Guest checks
        Assert.Equal(2, Result.Guests.Count());
        
        var guestIds = Result.Guests.Select(x => x.Id).ToList();

        Assert.Contains(Guest1.Id, guestIds);
        Assert.Contains(Guest2.Id, guestIds);
        
        await _fixture.Cleanup();
    }
}