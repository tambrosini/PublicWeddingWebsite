using WeddingInvites.Domain;

namespace WeddingInvites.Test.Invites;

[Collection("Tests")]
public class DeleteTests : IClassFixture<BaseFixture<Invite>>
{
    private readonly BaseFixture<Invite> _fixture;
    
    private Invite ToDelete { get; set; }
    
    private bool Result { get; set; }
    
    public DeleteTests(BaseFixture<Invite> fixture)
    {
        _fixture = fixture;
        _fixture.SetAuthCookie();
        
        _fixture.Setup = async () =>
        {
            ToDelete = new Invite()
            {
                Name = "Invite to delete",
                PublicCode = "123456",
                RsvpCompleted = false
            };
            
            await _fixture.TearUpAsync(ToDelete);
        };
        
        _fixture.Execute = async (_) =>
        {
            Result = await _fixture.DeleteAsync(ToDelete.Id);
        };
    }

    [Fact]
    public async Task GIVEN_Existing_Guest_WHEN_Delete_Is_Called_THEN_Returns_True()
    {
        await _fixture.Setup();

        await _fixture.Execute(null);
        
        Assert.True(Result);
        
        await _fixture.Cleanup();
    }
}