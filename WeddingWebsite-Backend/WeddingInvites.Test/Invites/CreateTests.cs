using WeddingInvites.Controllers.DTOs;
using WeddingInvites.Domain;

namespace WeddingInvites.Test.Invites;

[Collection("Tests")]
public class CreateTests : IClassFixture<BaseFixture<Invite>>
{
    private readonly BaseFixture<Invite> _fixture;
    
    private InviteCreateModel ToCreate { get; set; } = null!;
    
    private Invite Result { get; set; }
    
    public CreateTests(BaseFixture<Invite> fixture)
    {
        _fixture = fixture;
        _fixture.SetAuthCookie();

        _fixture.Setup = async () =>
        {
            // Nothing
        };
        
        _fixture.Execute = async (_) =>
        {
            Result = await _fixture.CreateAsync<InviteCreateModel, Invite>(ToCreate);
        };
    }
    
    [Fact]
    public async Task GIVEN_Create_Guest_When_Create_THEN_Result_Should_Be_Created()
    {
        ToCreate = new InviteCreateModel()
        {
            Name = "Test Invite"
        };
        
        await _fixture.Setup();

        await _fixture.Execute(null);
        
        Assert.False(Result.RsvpCompleted);
        Assert.True(!string.IsNullOrEmpty(Result.PublicCode));
        
        await _fixture.Cleanup();
    }
}