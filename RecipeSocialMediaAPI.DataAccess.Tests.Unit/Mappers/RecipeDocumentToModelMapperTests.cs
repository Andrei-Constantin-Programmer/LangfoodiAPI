using FluentAssertions;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapRecipeDocumentToRecipeAggregate_WhenDocumentIsValid_ReturnMappedRecipeModel()
    {
        // Given
        RecipeDocument testDocument = new()
        {
            Id = "RecipeId",
            Title = "Recipe Title",
            Description = "Recipe Description",
            CreationDate = new(2023, 11, 6, 0, 0, 0, TimeSpan.Zero),
            LastUpdatedDate = new(2023, 11, 6, 0, 0, 0, TimeSpan.Zero),
            Labels = new List<string>() { "Label1", "Label2" },
            ChefId = "ChefId",
            Ingredients = new List<(string, double, string)>()
            {
                new("Beef", 200, "g"),
                new("Onion", 150, "g"),
                new("Chicken Stock", 500, "mL"),
            },
            Steps = new List<(string, string?)>()
            {
                new("Chop onions", null),
                new("Put it all together", null),
                new("Boil for 10 minutes", null)
            },
            NumberOfServings = 2,
            CookingTimeInSeconds = 1200,
            KiloCalories = 300,
            ServingSize = (200, "g")
        };

        TestUserAccount testSender = new()
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "Test Username",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        List<Ingredient> expectedIngredients = new()
        {
            new("Beef", 200, "g"),
            new("Onion", 150, "g"),
            new("Chicken Stock", 500, "mL")
        };

        List<RecipeStep> expectedSteps = new()
        {
            new("Chop onions", null),
            new("Put it all together", null),
            new("Boil for 10 minutes", null)
        };

        // When
        var result= _recipeDocumentToModelMapperSUT.MapRecipeDocumentToRecipeAggregate(testDocument, testSender);

        // Then
        result.Id.Should().Be(testDocument.Id);
        result.Title.Should().Be(testDocument.Title);
        result.Description.Should().Be(testDocument.Description);
        result.CreationDate.Should().Be(testDocument.CreationDate);
        result.LastUpdatedDate.Should().Be(testDocument.LastUpdatedDate);
        result.Labels.Should().BeEquivalentTo(testDocument.Labels.ToHashSet());
        result.Chef.Should().Be(testSender);
        
        result.Recipe.Ingredients.Should().HaveCount(3);
        result.Recipe.Ingredients[0].Should().BeEquivalentTo(expectedIngredients[0]);
        result.Recipe.Ingredients[1].Should().BeEquivalentTo(expectedIngredients[1]);
        result.Recipe.Ingredients[2].Should().BeEquivalentTo(expectedIngredients[2]);

        result.Recipe.Steps.Should().HaveCount(3);
        var recipeSteps = result.Recipe.Steps.ToList();
        recipeSteps[0].Should().BeEquivalentTo(expectedSteps[0]);
        recipeSteps[1].Should().BeEquivalentTo(expectedSteps[1]);
        recipeSteps[2].Should().BeEquivalentTo(expectedSteps[2]);

        result.Recipe.CookingTimeInSeconds.Should().Be(testDocument.CookingTimeInSeconds);
        result.Recipe.NumberOfServings.Should().Be(testDocument.NumberOfServings);
        result.Recipe.KiloCalories.Should().Be(testDocument.KiloCalories);
        //result.Recipe.ServingSize.Should().BeEquivalentTo(new ServingSize(testDocument.ServingSize.Value.Quantity, testDocument.ServingSize.Value.UnitOfMeasurement));
    }
}
