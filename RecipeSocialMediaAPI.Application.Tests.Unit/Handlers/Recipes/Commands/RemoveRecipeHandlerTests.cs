using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Commands;
public class RemoveRecipeHandlerTests
{
    private readonly Mock<IRecipePersistenceRepository> _recipePersistenceRepositoryMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;

    private readonly RemoveRecipeHandler _removeRecipeHandler;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    public RemoveRecipeHandlerTests()
    {
        _recipePersistenceRepositoryMock = new Mock<IRecipePersistenceRepository>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();
        _removeRecipeHandler = new RemoveRecipeHandler(_recipePersistenceRepositoryMock.Object, _recipeQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RecipeDoesNotExist_ThrowRecipeNotFoundException()
    {
        // Given
        string recipeId = "1";

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<RecipeNotFoundException>()
            .WithMessage("The recipe with the id 1 was not found.");

        _recipePersistenceRepositoryMock
            .Verify(mapper => mapper.DeleteRecipe(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RecipeExistsButDeletionFailed_ThrowGeneralException()
    {
        // Given
        string recipeId = "1";

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeById(It.IsAny<string>()))
            .Returns(new RecipeAggregate(
                "1", "title",
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                "desc", new User("1", "user", "mail", "pass"),
                _testDate, _testDate
            ));

        _recipePersistenceRepositoryMock
            .Setup(x => x.DeleteRecipe(It.IsAny<string>()))
            .Returns(false);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Could not remove recipe with id 1.");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RecipeExistsAndIsDeleted_ReturnTaskCompleted()
    {
        // Given
        string recipeId = "1";

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeById(It.IsAny<string>()))
            .Returns(new RecipeAggregate(
                "1", "title",
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                "desc", new User("1", "user", "mail", "pass"),
                _testDate, _testDate
            ));

        _recipePersistenceRepositoryMock
            .Setup(x => x.DeleteRecipe(It.IsAny<string>()))
            .Returns(true);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should()
            .NotThrowAsync<RecipeNotFoundException>();

        await action.Should()
            .NotThrowAsync<Exception>();
    }
}
