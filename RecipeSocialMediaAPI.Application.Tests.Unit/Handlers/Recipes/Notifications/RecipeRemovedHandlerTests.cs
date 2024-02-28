using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Notifications;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Notifications;

public class RecipeRemovedHandlerTests
{
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;
    private readonly Mock<IMessagePersistenceRepository> _messagePersistenceRepositoryMock;

    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly IMessageFactory _messageFactory;

    private readonly RecipeRemovedHandler _recipeRemovedHandlerSUT;

    public RecipeRemovedHandlerTests()
    {
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();

        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _messageFactory = new MessageFactory(_dateTimeProviderMock.Object);

        _recipeRemovedHandlerSUT = new(_messageQueryRepositoryMock.Object, _messagePersistenceRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenNoMessagesFoundWithRecipe_DoNothing()
    {
        // Given
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessagesWithRecipe(It.IsAny<string>()))
            .Returns(Enumerable.Empty<Message>());

        // When
        await _recipeRemovedHandlerSUT.Handle(new RecipeRemovedNotification("r1"), CancellationToken.None);

        // Then
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.DeleteMessage(It.IsAny<Message>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenMessagesFoundWithRecipe_DeleteThoseWithNoOtherRecipesOrTextContent()
    {
        // Given
        IUserAccount testSender = new TestUserAccount()
        {
            Id = "u1",
            UserName = "User",
            Handler = "user_1"
        };

        RecipeAggregate recipeBeingDeleted = new(
            "r1",
            "Recipe1",
            new(new(), new()),
            "Description1",
            testSender,
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));

        RecipeAggregate recipeNotBeingDeleted = new(
            "r2",
            "Recipe2",
            new(new(), new()),
            "Description2",
            testSender,
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));
        
        var message1 = (RecipeMessage)_messageFactory.CreateRecipeMessage(
            "m1",
            testSender,
            new List<RecipeAggregate> { recipeBeingDeleted },
            "Has Text Content",
            new(),
            new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            );

        var message2 = (RecipeMessage)_messageFactory.CreateRecipeMessage(
            "m2",
            testSender,
            new List<RecipeAggregate> { recipeBeingDeleted, recipeNotBeingDeleted },
            null,
            new(),
            new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            );

        var message3 = (RecipeMessage)_messageFactory.CreateRecipeMessage(
            "m3",
            testSender,
            new List<RecipeAggregate> { recipeBeingDeleted },
            null,
            new(),
            new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            );

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessagesWithRecipe(It.IsAny<string>()))
            .Returns(new List<Message> { message1, message2, message3 });

        // When
        await _recipeRemovedHandlerSUT.Handle(new RecipeRemovedNotification(recipeBeingDeleted.Id), CancellationToken.None);

        // Then
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.DeleteMessage(It.IsAny<Message>()), Times.Once);
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.DeleteMessage(It.Is<Message>(m => m == message3)), Times.Once);
    }
}
