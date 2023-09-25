using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Core.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Recipes.Commands;
public class UpdateRecipeHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;
    private readonly Mock<IDateTimeProvider> _timeProviderMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly UpdateRecipeHandler _updateRecipeHandlerSUT;

    public UpdateRecipeHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _timeProviderMock = new Mock<IDateTimeProvider>();

        _timeProviderMock
            .Setup(x => x.Now)
            .Returns(_testDate);

        _updateRecipeHandlerSUT = new UpdateRecipeHandler(
            _recipeMapperMock.Object,
            _recipeRepositoryMock.Object,
            _timeProviderMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenRecipeDoesNotExist_ThrowRecipeNotFoundException()
    {
        // Given
        UpdateRecipeContract testContract = new UpdateRecipeContract()
        {
            Id = "1",
            Title = "Test",
            Description = "Test",            
            Labels = new HashSet<string>(),
            Ingredients = new List<IngredientDTO>(),
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        // When
        var action = async () => await _updateRecipeHandlerSUT.Handle(new UpdateRecipeCommand(testContract), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<RecipeNotFoundException>()
            .WithMessage("The recipe with the id 1 was not found.");

        _recipeRepositoryMock
            .Verify(mapper => mapper.UpdateRecipe(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenRecipeExistsButUpdateFails_ThrowNewGeneralException()
    {
        // Given
        UpdateRecipeContract testContract = new UpdateRecipeContract()
        {
            Id = "1",
            Title = "Test",
            Description = "Test",
            Labels = new HashSet<string>(),
            Ingredients = new List<IngredientDTO>(),
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        _recipeRepositoryMock
            .Setup(x => x.GetRecipeById(It.IsAny<string>()))
            .Returns(new RecipeAggregate(
                testContract.Id, testContract.Title,
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                testContract.Description, new User("1", "user", "mail", "pass"),
                _testDate, _testDate
            ));

        _recipeRepositoryMock
            .Setup(x => x.UpdateRecipe(It.IsAny<RecipeAggregate>()))
            .Returns(false);

        // When
        var action = async () => await _updateRecipeHandlerSUT.Handle(new UpdateRecipeCommand(testContract), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Could not update recipe with id 1.");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenRecipeExistsAndUpdateSucceeds_ReturnsCompletedTask()
    {
        // Given
        UpdateRecipeContract testContract = new UpdateRecipeContract()
        {
            Id = "1",
            Title = "Test",
            Description = "Test",
            Labels = new HashSet<string>(),
            Ingredients = new List<IngredientDTO>(),
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        _recipeRepositoryMock
            .Setup(x => x.GetRecipeById(It.IsAny<string>()))
            .Returns(new RecipeAggregate(
                testContract.Id, testContract.Title,
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                testContract.Description, new User("1", "user", "mail", "pass"),
                _testDate, _testDate
            ));

        _recipeRepositoryMock
            .Setup(x => x.UpdateRecipe(It.IsAny<RecipeAggregate>()))
            .Returns(true);

        // When
        var action = async () => await _updateRecipeHandlerSUT.Handle(new UpdateRecipeCommand(testContract), CancellationToken.None);

        // Then
        await action.Should()
            .NotThrowAsync<RecipeNotFoundException>();

        await action.Should()
            .NotThrowAsync<Exception>();
    }
}
