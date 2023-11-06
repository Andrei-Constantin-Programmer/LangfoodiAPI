using FluentAssertions;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Mappers;

public class RecipeDocumentToModelMapperTests
{
    private readonly RecipeDocumentToModelMapper _recipeDocumentToModelMapperSUT;

    public RecipeDocumentToModelMapperTests()
    {
        _recipeDocumentToModelMapperSUT = new RecipeDocumentToModelMapper();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapRecipeDocumentToRecipeAggregate_WhenIdIsNull_ThrowArgumentException()
    {
        // Given
        RecipeDocument testDocument = new()
        {
            Id = null,
            Title = "Recipe Title",
            Description = "Recipe Description",
            CreationDate = new(2023, 11, 6, 0, 0, 0, TimeSpan.Zero),
            LastUpdatedDate = new(2023, 11, 6, 0, 0, 0, TimeSpan.Zero),
            Labels = new List<string>(),
            ChefId = "ChefId",
            Ingredients = new List<(string, double, string)>(),
            Steps = new List<(string, string?)>()
        };

        TestUserAccount testSender = new()
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "Test Username",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        // When
        var testAction = () => _recipeDocumentToModelMapperSUT.MapRecipeDocumentToRecipeAggregate(testDocument, testSender);

        // Then
        testAction.Should().Throw<ArgumentException>();
    }
}
