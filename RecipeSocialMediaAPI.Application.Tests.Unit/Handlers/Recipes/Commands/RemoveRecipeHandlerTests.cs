using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Notifications;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Commands;

public class RemoveRecipeHandlerTests
{
    private readonly Mock<ILogger<RemoveRecipeCommand>> _loggerMock;
    private readonly Mock<ICloudinaryWebClient> _cloudinaryWebClientMock;
    private readonly Mock<IRecipePersistenceRepository> _recipePersistenceRepositoryMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;
    private readonly Mock<IPublisher> _publisherMock;

    private readonly RemoveRecipeHandler _removeRecipeHandler;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    public RemoveRecipeHandlerTests()
    {
        _loggerMock = new Mock<ILogger<RemoveRecipeCommand>>();
        _recipePersistenceRepositoryMock = new Mock<IRecipePersistenceRepository>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();
        _cloudinaryWebClientMock = new Mock<ICloudinaryWebClient>();
        _publisherMock = new Mock<IPublisher>();

        _removeRecipeHandler = new RemoveRecipeHandler(
            _recipePersistenceRepositoryMock.Object,
            _recipeQueryRepositoryMock.Object,
            _cloudinaryWebClientMock.Object,
            _loggerMock.Object,
            _publisherMock.Object);
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
            .Verify(repo => repo.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RecipeExistsButDeletionFailed_ThrowRecipeRemovalException()
    {
        // Given
        string recipeId = "1";

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Recipe(
                "1", 
                "title",
                new RecipeGuide(new List<Ingredient>(), new Stack<RecipeStep>()),
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
            .Setup(x => x.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<RecipeRemovalException>()
            .WithMessage($"*{recipeId}*");

        _recipePersistenceRepositoryMock
            .Verify(repo => repo.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RecipeExistsAndIsDeleted_ReturnTaskCompleted()
    {
        // Given
        string recipeId = "1";

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Recipe(
                "1", 
                "title",
                new RecipeGuide(new List<Ingredient>(), new Stack<RecipeStep>()),
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
            .Setup(x => x.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();

        _recipePersistenceRepositoryMock
            .Verify(repo => repo.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RecipeAndImagesExistAndAreSuccesfullyDeleted_NoImageRemovalFailureLogAndReturnTaskCompleted()
    {
        // Given
        string recipeId = "1";
        Stack<RecipeStep> testRecipeSteps = new();
        testRecipeSteps.Push(new RecipeStep("step1", new RecipeImage("step1_img_id")));
        testRecipeSteps.Push(new RecipeStep("step2", null));

        Recipe testRecipe = new(
            recipeId,
            "title",
            new RecipeGuide(new List<Ingredient>(), testRecipeSteps),
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
            .Setup(x => x.GetRecipeByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testRecipe);

        _recipePersistenceRepositoryMock
            .Setup(x => x.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(It.IsAny<List<string>>()))
            .Returns(true);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();

        _recipePersistenceRepositoryMock
            .Verify(repo => repo.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _cloudinaryWebClientMock
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
        Stack<RecipeStep> testRecipeSteps = new();
        testRecipeSteps.Push(new RecipeStep("step1", new RecipeImage("step1_img_id")));
        testRecipeSteps.Push(new RecipeStep("step2", null));

        Recipe testRecipe = new(
            recipeId,
            "title",
            new RecipeGuide(new List<Ingredient>(), testRecipeSteps),
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
            .Setup(x => x.GetRecipeByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testRecipe);

        _recipePersistenceRepositoryMock
            .Setup(x => x.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(It.IsAny<List<string>>()))
            .Returns(false);

        // When
        var action = async () => await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();

        _recipePersistenceRepositoryMock
            .Verify(repo => repo.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _cloudinaryWebClientMock
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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RecipeExistsAndIsDeleted_PublishRecipeRemovedNotification()
    {
        // Given
        string recipeId = "1";

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Recipe(
                "1",
                "title",
                new RecipeGuide(new List<Ingredient>(), new Stack<RecipeStep>()),
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
            .Setup(x => x.DeleteRecipeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        await _removeRecipeHandler.Handle(new RemoveRecipeCommand(recipeId), CancellationToken.None);

        // Then
        _publisherMock
            .Verify(publisher => publisher.Publish(
                    It.Is<RecipeRemovedNotification>(notification => notification.RecipeId == recipeId), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }
}
