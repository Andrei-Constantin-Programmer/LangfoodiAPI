using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.Contracts.Recipes;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Core.Mappers.Recipes;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Recipes.Commands;
public class AddRecipeHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;
    private readonly Mock<IRecipeAggregateToRecipeDetailedDtoMapper> _recipeAggregateToRecipeDetailedDtoMapperMock;
    private readonly Mock<IIngredientMapper> _ingredientMapperMock;
    private readonly Mock<IRecipeStepMapper> _recipeStepMapperMock;
    private readonly Mock<IDateTimeProvider> _timeProviderMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly AddRecipeHandler _addRecipeHandlerSUT;

    public AddRecipeHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _timeProviderMock = new Mock<IDateTimeProvider>();
        _ingredientMapperMock = new Mock<IIngredientMapper>();
        _recipeStepMapperMock = new Mock<IRecipeStepMapper>();
        _recipeAggregateToRecipeDetailedDtoMapperMock = new Mock<IRecipeAggregateToRecipeDetailedDtoMapper>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();

        _recipeMapperMock
            .Setup(x => x.RecipeAggregateToRecipeDetailedDtoMapper)
            .Returns(_recipeAggregateToRecipeDetailedDtoMapperMock.Object);

        _recipeMapperMock
            .Setup(x => x.IngredientMapper)
            .Returns(_ingredientMapperMock.Object);

        _recipeMapperMock
            .Setup(x => x.RecipeStepMapper)
            .Returns(_recipeStepMapperMock.Object);

        _timeProviderMock
            .Setup(x => x.Now)
            .Returns(_testDate);

        _addRecipeHandlerSUT = new AddRecipeHandler(
            _recipeMapperMock.Object,
            _userRepositoryMock.Object,
            _recipeRepositoryMock.Object,
            _timeProviderMock.Object
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserDoesNotExist_ThrowUserNotFoundException()
    {
        // Given
        NewRecipeContract testContract = new NewRecipeContract()
        {
            Title = "Test",
            Description = "Test",
            ChefId = "1",
            Labels = new HashSet<string>(),
            Ingredients = new List<IngredientDTO>(),
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        // When
        var action = async () => await _addRecipeHandlerSUT.Handle(new AddRecipeCommand(testContract), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<UserNotFoundException>();

        _recipeAggregateToRecipeDetailedDtoMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserExists_CreateAndReturnRecipe()
    {
        // Given
        NewRecipeContract testContract = new NewRecipeContract()
        {
            Title = "Test",
            Description = "Test",
            ChefId = "1",
            Labels = new HashSet<string>(),
            NumberOfServings = 1,
            KiloCalories = 2300,
            CookingTime = 500,
            Ingredients = new List<IngredientDTO>() {
                new IngredientDTO()
                {
                    Name = "eggs",
                    Quantity = 1,
                    UnitOfMeasurement = "whole"
                }
            },
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        testContract.RecipeSteps.Push(new RecipeStepDTO()
        {
            Text = "step",
            ImageUrl = "url"
        });

        _userRepositoryMock
            .Setup(x => x.GetUserById(It.IsAny<string>()))
            .Returns(new User("1", "user", "mail", "pass"));

        _recipeRepositoryMock
            .Setup(x => x.CreateRecipe(It.IsAny<string>(), It.IsAny<Recipe>(), It.IsAny<string>(),
                It.IsAny<User>(), It.IsAny<ISet<string>>(), It.IsAny<int?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTimeOffset>(),
                It.IsAny<DateTimeOffset>()))
            .Returns((
                string title, Recipe recipe, string desc, 
                User chef, ISet<string> labels, int? numberOfServings,
                int? cookingTime, int? kiloCalories, DateTimeOffset creationDate,
                DateTimeOffset lastUpdatedDate) => new RecipeAggregate(
                    "1", title, recipe, desc, chef, creationDate, lastUpdatedDate,
                    labels, numberOfServings, cookingTime, kiloCalories
                )
            );

        _ingredientMapperMock
            .Setup(x => x.MapIngredientDtoToIngredient(It.IsAny<IngredientDTO>()))
            .Returns((IngredientDTO ing) => new Ingredient(ing.Name, ing.Quantity, ing.UnitOfMeasurement));

        _recipeStepMapperMock
            .Setup(x => x.MapRecipeStepDtoToRecipeStep(It.IsAny<RecipeStepDTO>()))
            .Returns((RecipeStepDTO step) => new RecipeStep(step.Text, new RecipeImage(step.ImageUrl)));

        _recipeAggregateToRecipeDetailedDtoMapperMock
            .Setup(x => x.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()))
            .Returns((RecipeAggregate recipe) => new RecipeDetailedDTO() {
                Id = "1", Title = recipe.Title, Description = recipe.Description,
                Chef = new UserDTO() { Id = "1", Email = "mail", UserName = "name", Password = "pass" },
                NumberOfServings = recipe.NumberOfServings, CookingTime = recipe.CookingTimeInSeconds,
                KiloCalories = recipe.KiloCalories, Labels = recipe.Labels, CreationDate = recipe.CreationDate,
                LastUpdatedDate = recipe.LastUpdatedDate, 
                Ingredients = ImmutableList.CreateRange(new List<IngredientDTO>() {
                    new IngredientDTO() { 
                        Name = recipe.Recipe.Ingredients.First().Name,
                        Quantity = recipe.Recipe.Ingredients.First().Quantity,
                        UnitOfMeasurement = recipe.Recipe.Ingredients.First().UnitOfMeasurement
                    }
                }),
                RecipeSteps = ImmutableStack.CreateRange(new List<RecipeStepDTO>() {
                    new RecipeStepDTO() {
                        Text = recipe.Recipe.Steps.First().Text,
                        ImageUrl = recipe.Recipe.Steps.First().Image.ImageUrl
                    }
                })
            });

        // When
        var result = await _addRecipeHandlerSUT.Handle(new AddRecipeCommand(testContract), CancellationToken.None);

        // Then
        result.Title.Should().Be("Test");
        result.Description.Should().Be("Test");
        result.NumberOfServings.Should().Be(1);
        result.CookingTime.Should().Be(500);
        result.KiloCalories.Should().Be(2300);
        result.Ingredients.First().Name.Should().Be("eggs");
        result.Ingredients.First().Quantity.Should().Be(1);
        result.Ingredients.First().UnitOfMeasurement.Should().Be("whole");
        result.RecipeSteps.First().Text.Should().Be("step");
        result.RecipeSteps.First().ImageUrl.Should().Be("url");
        result.CreationDate.Should().Be(_testDate);
        result.LastUpdatedDate.Should().Be(_testDate);
        _recipeAggregateToRecipeDetailedDtoMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()), Times.Once);
    }
}
