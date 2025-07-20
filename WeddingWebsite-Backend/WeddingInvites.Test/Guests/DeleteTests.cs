using WeddingInvites.Domain;

namespace WeddingInvites.Test.Guests;

[Collection("Tests")]
public class DeleteTests : IClassFixture<BaseFixture<Guest>>
{
    private readonly BaseFixture<Guest> _fixture;
    
    private Guest ToDelete { get; set; }
    
    private bool Result { get; set; }

    public DeleteTests(BaseFixture<Guest> fixture)
    {
        _fixture = fixture;
        _fixture.SetAuthCookie();
        
        _fixture.Setup = async () =>
        {
            ToDelete = new Guest()
            {
                FirstName = "John",
                LastName = "Doe",
                Attending = true,
                DietaryRequirements = "Eggs"
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