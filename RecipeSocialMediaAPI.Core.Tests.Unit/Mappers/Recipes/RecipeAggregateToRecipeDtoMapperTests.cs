using FluentAssertions;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Mappers.Recipes.Mappers;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Mappers.Recipes;
public class RecipeAggregateToRecipeDtoMapperTests
{
    private readonly RecipeAggregateToRecipeDtoMapper _mapperSUT;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    public RecipeAggregateToRecipeDtoMapperTests()
    {
        _mapperSUT = new RecipeAggregateToRecipeDtoMapper();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MapRecipeAggregateToRecipeDto_GivenRecipeAggregate_ReturnRecipeDto()
    {
        // Given
        User testChef = new("1", "TestChef", "chef@mail.com", "TestPass");
        RecipeAggregate testRecipe = new(
            "1",
            "title",
            new Recipe(new(), new()),
            "desc",
            testChef,
            _testDate,
            _testDate,
            new HashSet<string>(),
            1);

        // When
        var result = _mapperSUT.MapRecipeAggregateToRecipeDto(testRecipe);

        // Then
        result.Should().BeOfType<RecipeDTO>();
        result.Id.Should().Be("1");
        result.Title.Should().Be("title");
        result.Description.Should().Be("desc");
        result.ChefUsername.Should().Be("TestChef");
        result.CreationDate.Should().Be(_testDate);
        result.NumberOfServings.Should().Be(1);
        result.Labels.Should().BeEmpty();
        result.CookingTime.Should().BeNull();
        result.KiloCalories.Should().BeNull();
    }

}
