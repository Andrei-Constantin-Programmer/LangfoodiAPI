using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Mappers.Recipes.Mappers;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Mappers.Recipes;
public class RecipeAggregateToRecipeDetailedDtoMapperTests
{
    private readonly Mock<IIngredientMapper> _ingredientMapperMock;
    private readonly Mock<IRecipeStepMapper> _recipeStepMapperMock;
    private readonly Mock<IUserMapper> _userMapperMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly RecipeAggregateToRecipeDetailedDtoMapper _mapperSUT;

    public RecipeAggregateToRecipeDetailedDtoMapperTests()
    {
        _ingredientMapperMock = new Mock<IIngredientMapper>();
        _recipeStepMapperMock = new Mock<IRecipeStepMapper>();
        _userMapperMock = new Mock<IUserMapper>();
        _mapperSUT = new RecipeAggregateToRecipeDetailedDtoMapper(
            _userMapperMock.Object,
            _ingredientMapperMock.Object,
            _recipeStepMapperMock.Object
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MapRecipeAggregateToRecipeDetailedDto_GivenRecipeAggregate_ReturnRecipeDetailedDto()
    {
        // Given
        User testChef = new("1", "TestChef", "chef@mail.com", "TestPass");
        RecipeAggregate testRecipe = new(
            "1",
            "title",
            new Recipe(new() { 
                new Ingredient("eggs", 1, "whole") },
                new(new[] { new RecipeStep("step1", new RecipeImage("url"))})),
            "desc",
            testChef,
            _testDate,
            _testDate,
            new HashSet<string>(),
            1);

        _userMapperMock
            .Setup(x => x.MapUserToUserDto(It.IsAny<User>()))
            .Returns((User user) => new UserDTO() { 
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Password = user.Password 
            });

        _ingredientMapperMock
            .Setup(x => x.MapIngredientToIngredientDto(It.IsAny<Ingredient>()))
            .Returns((Ingredient ing) => new IngredientDTO() {
                Name = ing.Name,
                Quantity = ing.Quantity,
                UnitOfMeasurement = ing.UnitOfMeasurement,
            });

        _recipeStepMapperMock
            .Setup(x => x.MapRecipeStepToRecipeStepDto(It.IsAny<RecipeStep>()))
            .Returns((RecipeStep step) => new RecipeStepDTO()
            {
                Text = step.Text,
                ImageUrl = step.Image.ImageUrl
            });

        // When
        var result = _mapperSUT.MapRecipeAggregateToRecipeDetailedDto(testRecipe);

        // Then
        result.Should().BeOfType<RecipeDetailedDTO>();
        result.Id.Should().Be("1");
        result.Title.Should().Be("title");
        result.Description.Should().Be("desc");
        result.Chef.Should().BeEquivalentTo(testChef);
        result.CreationDate.Should().Be(_testDate);
        result.LastUpdatedDate.Should().Be(_testDate);
        result.Labels.Should().BeEmpty();
        result.NumberOfServings.Should().Be(1);
        result.CookingTime.Should().BeNull();
        result.KiloCalories.Should().BeNull();

        result.Ingredients.First().Name.Should().Be("eggs");
        result.Ingredients.First().Quantity.Should().Be(1);
        result.Ingredients.First().UnitOfMeasurement.Should().Be("whole");

        result.RecipeSteps.First().Text.Should().Be("step1");
        result.RecipeSteps.First().ImageUrl.Should().Be("url");
    }

}
