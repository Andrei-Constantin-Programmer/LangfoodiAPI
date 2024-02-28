using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

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
        var messages = _messageQueryRepository.GetMessagesWithRecipe(notification.RecipeId);

        foreach (var message in messages.Cast<RecipeMessage>())
        {
            if (message.Recipes.Count == 1
                && string.IsNullOrWhiteSpace(message.TextContent))
            {
                _messagePersistenceRepository.DeleteMessage(message);
            }
        }

        return Task.CompletedTask;
    }
}
