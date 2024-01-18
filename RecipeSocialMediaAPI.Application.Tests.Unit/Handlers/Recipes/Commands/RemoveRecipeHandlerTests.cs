using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Images;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Commands;
public class RemoveRecipeHandlerTests
{
    private readonly Mock<ILogger<RemoveRecipeCommand>> _loggerMock;
    private readonly Mock<IImageHostingPersistenceRepository> _imageHostingPersistenceRepositoryMock;
    private readonly Mock<IRecipePersistenceRepository> _recipePersistenceRepositoryMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;

    private readonly RemoveRecipeHandler _removeRecipeHandler;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    public RemoveRecipeHandlerTests()
    {
        _loggerMock = new Mock<ILogger<RemoveRecipeCommand>>();
        _recipePersistenceRepositoryMock = new Mock<IRecipePersistenceRepository>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();
        _imageHostingPersistenceRepositoryMock = new Mock<IImageHostingPersistenceRepository>();
        _removeRecipeHandler = new RemoveRecipeHandler(
            _recipePersistenceRepositoryMock.Object,
            _recipeQueryRepositoryMock.Object,
            _imageHostingPersistenceRepositoryMock.Object,
            _loggerMock.Object);
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
            .WithMessage("The recipe with the id 1 was not found");

        _recipePersistenceRepositoryMock
            .Verify(repo => repo.DeleteRecipe(It.IsAny<string>()), Times.Never);
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
                "1", 
                "title",
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                "desc", 
                new TestUserAccount
                {
                    Id = "1",
                    Handler = "handler",
                    UserName = "name",
                    AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
                },
                _testDate, 
                _testDate
            ));

        _recipePersistenceRepositoryMock
            .Setup(x => x.DeleteRecipe(It.IsAny<string>()))
            .Returns(false);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<RecipeRemovalException>()
            .WithMessage($"*{recipeId}*");

        _recipePersistenceRepositoryMock
            .Verify(repo => repo.DeleteRecipe(It.IsAny<string>()), Times.Once);
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
                "1", 
                "title",
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                "desc", 
                new TestUserAccount
                {
                    Id = "1",
                    Handler = "handler",
                    UserName = "name",
                    AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
                },
                _testDate, 
                _testDate,
                new HashSet<string>()
            ));

        _recipePersistenceRepositoryMock
            .Setup(x => x.DeleteRecipe(It.IsAny<string>()))
            .Returns(true);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();

        _recipePersistenceRepositoryMock
            .Verify(repo => repo.DeleteRecipe(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RecipeAndImagesExistAndAreSuccesfullyDeleted_NoImageRemovalFailureLogAndReturnTaskCompleted()
    {
        // Given
        string recipeId = "1";
        Stack<RecipeStep> testRecipeSteps = new Stack<RecipeStep>();
        testRecipeSteps.Push(new RecipeStep("step1", new RecipeImage("step1_img_id")));
        testRecipeSteps.Push(new RecipeStep("step2", null));

        RecipeAggregate testRecipe = new RecipeAggregate(
            recipeId,
            "title",
            new Recipe(new List<Ingredient>(), testRecipeSteps),
            "desc",
            new TestUserAccount
            {
                Id = "1",
                Handler = "handler",
                UserName = "name",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            _testDate,
            _testDate,
            new HashSet<string>(),
            "thumbnail_id_1"
        );

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeById(It.IsAny<string>()))
            .Returns(testRecipe);

        _recipePersistenceRepositoryMock
            .Setup(x => x.DeleteRecipe(It.IsAny<string>()))
            .Returns(true);

        _imageHostingPersistenceRepositoryMock
            .Setup(x => x.BulkRemoveHostedImages(It.IsAny<List<string>>()))
            .Returns(true);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();

        _recipePersistenceRepositoryMock
            .Verify(repo => repo.DeleteRecipe(It.IsAny<string>()), Times.Once);

        _imageHostingPersistenceRepositoryMock
            .Verify(repo => repo.BulkRemoveHostedImages(new() { "step1_img_id", "thumbnail_id_1" }), Times.Once);

        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RecipeExistsAndImagesExistButOnlyRecipeIsSuccessfullyDeleted_LogImageRemovalFailureAndReturnTaskCompleted()
    {
        // Given
        string recipeId = "1";
        Stack<RecipeStep> testRecipeSteps = new Stack<RecipeStep>();
        testRecipeSteps.Push(new RecipeStep("step1", new RecipeImage("step1_img_id")));
        testRecipeSteps.Push(new RecipeStep("step2", null));

        RecipeAggregate testRecipe = new RecipeAggregate(
            recipeId,
            "title",
            new Recipe(new List<Ingredient>(), testRecipeSteps),
            "desc",
            new TestUserAccount
            {
                Id = "1",
                Handler = "handler",
                UserName = "name",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            _testDate,
            _testDate,
            new HashSet<string>(),
            "thumbnail_id_1"
        );

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeById(It.IsAny<string>()))
            .Returns(testRecipe);

        _recipePersistenceRepositoryMock
            .Setup(x => x.DeleteRecipe(It.IsAny<string>()))
            .Returns(true);

        _imageHostingPersistenceRepositoryMock
            .Setup(x => x.BulkRemoveHostedImages(It.IsAny<List<string>>()))
            .Returns(false);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();

        _recipePersistenceRepositoryMock
            .Verify(repo => repo.DeleteRecipe(It.IsAny<string>()), Times.Once);

        _imageHostingPersistenceRepositoryMock
            .Verify(repo => repo.BulkRemoveHostedImages(new() { "step1_img_id", "thumbnail_id_1" }), Times.Once);

        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

}
