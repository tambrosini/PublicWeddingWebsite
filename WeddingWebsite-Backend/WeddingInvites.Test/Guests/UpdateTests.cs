using WeddingInvites.Domain;

namespace WeddingInvites.Test.Guests;

[Collection("Tests")]
public class UpdateTests : IClassFixture<BaseFixture<Guest>>
{
    private readonly BaseFixture<Guest> _fixture;
    
    private Guest ToUpdate { get; set; }
    
    private Guest Result { get; set; }

    public UpdateTests(BaseFixture<Guest> fixture)
    {
        _fixture = fixture;
        _fixture.SetAuthCookie();

        _fixture.Setup = async () =>
        {
            ToUpdate = new Guest
            {
                FirstName = "John",
                LastName = "Wick",
                Attending = false
            };

            await _fixture.TearUpAsync(ToUpdate);
        };

        _fixture.Execute = async (entity) =>
        {
            Result = await _fixture.UpdateAsync(entity);
        };


    }

    [Fact]
    public async Task GIVEN_Update_Guest_THEN_Result_values_should_match()
    {
        await _fixture.Setup();
        
        ToUpdate.Attending = true;
        ToUpdate.DietaryRequirements= "Carnivore";
        
        await _fixture.Execute(ToUpdate);
        
        Assert.Equal(ToUpdate.DietaryRequirements, Result.DietaryRequirements);
        Assert.True(Result.Attending);
    }
    
    [Fact]
    public async Task GIVEN_Update_Id_doesnt_exist_THEN_should_fail()
    {
        await _fixture.Setup();
        
        var failureModel = new Guest
        {
            FirstName = "Jessica",
            LastName = "Brown",
            Id = 9999999,
        };

        await Assert.ThrowsAsync<HttpRequestException>(async () => await _fixture.Execute(failureModel));
    }
    
    [Fact]
    public async Task GIVEN_Update_InviteId_doesnt_exist_THEN_should_fail()
    {
        await _fixture.Setup();
        
        var failureModel = new Guest
        {
            FirstName = "Jessica",
            LastName = "Brown",
            Id = ToUpdate.Id,
            InviteId = 9999999,
        };

        await Assert.ThrowsAsync<HttpRequestException>(async () => await _fixture.Execute(failureModel));
    }
}