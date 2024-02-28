using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Notifications;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Notifications;

public class RecipeRemovedHandlerTests
{
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;
    private readonly Mock<IMessagePersistenceRepository> _messagePersistenceRepositoryMock;

    private readonly RecipeRemovedHandler _recipeRemovedHandlerSUT;

    public RecipeRemovedHandlerTests()
    {
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();

        _recipeRemovedHandlerSUT = new(_messageQueryRepositoryMock.Object, _messagePersistenceRepositoryMock.Object);
    }

}
