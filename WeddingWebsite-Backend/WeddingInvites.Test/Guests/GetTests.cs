using Azure;
using WeddingInvites.Domain;

namespace WeddingInvites.Test.Guests;

[Collection("Tests")]
public class GetTests : IClassFixture<BaseFixture<Guest>>
{
    private readonly BaseFixture<Guest> _fixture;
    
    private Guest ToGet { get; set; }
    
    private Guest ToGet2 { get; set; }
    
    private List<Guest> ListResult { get; set; }
    
    private Guest SingleResult { get; set; }

    public GetTests(BaseFixture<Guest> fixture)
    {
        _fixture = fixture;
        _fixture.SetAuthCookie();

        _fixture.Setup = async () =>
        {
            ToGet = new Guest()
            {
                FirstName = "John",
                LastName = "Doe",
                Attending = true,
                DietaryRequirements = "Eggs"
            };

            ToGet2 = new Guest()
            {
                FirstName = "Jane",
                LastName = "Smith",
                Attending = false,
                DietaryRequirements = "Eggs"
            };

            await _fixture.TearUpAsync([ToGet, ToGet2]);
        };

        _fixture.Execute = async (_) =>
        {
            SingleResult = await _fixture.GetByIdAsync(ToGet.Id);

            ListResult = await _fixture.ListAsync<Guest>();
        };
    }

    [Fact]
    public async Task GIVEN_Guests_Exist_When_GetByIdAsync_AND_List_THEN_Results_Returned()
    {
        await _fixture.Setup();

        await _fixture.Execute(null);
        
        // Single Result
        Assert.Equal(ToGet.Id, SingleResult.Id);
        Assert.Equal(ToGet.FirstName, SingleResult.FirstName);
        Assert.Equal(ToGet.LastName, SingleResult.LastName);
        Assert.Equal(ToGet.Attending, SingleResult.Attending);
        Assert.Equal(ToGet.DietaryRequirements, SingleResult.DietaryRequirements);
        
        // List Results
        Assert.True(ListResult.Count >= 2);
        
        var guest1 = ListResult.Single(x => x.Id == ToGet.Id);
        var guest2 = ListResult.Single(x => x.Id == ToGet2.Id);
        
        Assert.Equal(ToGet.Id, guest1.Id);
        Assert.Equal(ToGet.FirstName, guest1.FirstName);
        Assert.Equal(ToGet.LastName, guest1.LastName);
        Assert.Equal(ToGet.Attending, guest1.Attending);
        Assert.Equal(ToGet.DietaryRequirements, guest1.DietaryRequirements);
        Assert.Equal(ToGet2.Id, guest2.Id);
        Assert.Equal(ToGet2.FirstName, guest2.FirstName);
        Assert.Equal(ToGet2.LastName, guest2.LastName);
        Assert.Equal(ToGet2.Attending, guest2.Attending);
        Assert.Equal(ToGet2.DietaryRequirements, guest2.DietaryRequirements);


        
        await _fixture.Cleanup();
    }
}