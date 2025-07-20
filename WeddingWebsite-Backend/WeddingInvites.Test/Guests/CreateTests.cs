using WeddingInvites.Domain;
 
namespace WeddingInvites.Test.Guests;

[Collection("Tests")]
public class CreateTests : IClassFixture<BaseFixture<Guest>>
{
    private readonly BaseFixture<Guest> _fixture;
    
    private Guest ToCreate { get; set; } = null!;
    
    private Guest Result { get; set; }

    public CreateTests(BaseFixture<Guest> fixture)
    {
        _fixture = fixture;
        _fixture.SetAuthCookie();

        _fixture.Setup = async () =>
        {
            // Nothing
        };
        
        _fixture.Execute = async (_) =>
        {
            Result = await _fixture.CreateAsync(ToCreate);
        };
    }

    [Fact]
    public async Task GIVEN_Create_Guest_When_Create_THEN_Result_Should_Be_Created()
    {
        ToCreate = new Guest()
        {
            FirstName = "John",
            LastName = "Smith",
            Attending = true
        };

        await _fixture.Execute(null);
        
        Assert.Equal(ToCreate.FirstName, Result.FirstName);
        Assert.Equal(ToCreate.LastName, Result.LastName);
        Assert.Null(Result.Attending);
        Assert.Null(Result.InviteId);
        Assert.True(string.IsNullOrEmpty(Result.DietaryRequirements));
        
        await _fixture.Cleanup();
    }
}