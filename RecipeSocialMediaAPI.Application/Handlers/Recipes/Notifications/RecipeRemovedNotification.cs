using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Notifications;

public record RecipeRemovedNotification(string RecipeId) : INotification;

internal class RecipeRemovedHandler : INotificationHandler<RecipeRemovedNotification>
{
    private readonly IMessageQueryRepository _messageQueryRepository;
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;

    public RecipeRemovedHandler(IMessageQueryRepository messageQueryRepository, IMessagePersistenceRepository messagePersistenceRepository)
    {
        _messageQueryRepository = messageQueryRepository;
        _messagePersistenceRepository = messagePersistenceRepository;
    }

    public Task Handle(RecipeRemovedNotification notification, CancellationToken cancellationToken)
    {
        // TODO: To be implemented
        return Task.CompletedTask;
    }
}
