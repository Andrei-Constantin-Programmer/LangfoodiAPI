using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record MarkMessageAsReadCommand(string UserId, string MessageId) : IRequest;

internal class MarkMessageAsReadHandler : IRequestHandler<MarkMessageAsReadCommand>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IPublisher _publisher;

    public MarkMessageAsReadHandler(IUserQueryRepository userQueryRepository, IMessageQueryRepository messageQueryRepository, IMessagePersistenceRepository messagePersistenceRepository, IPublisher publisher)
    {
        _userQueryRepository = userQueryRepository;
        _messageQueryRepository = messageQueryRepository;
        _messagePersistenceRepository = messagePersistenceRepository;
        _publisher = publisher;
    }

    public async Task Handle(MarkMessageAsReadCommand request, CancellationToken cancellationToken)
    {
        var user = (await _userQueryRepository.GetUserById(request.UserId, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No User found with id {request.UserId}");
        var message = (await _messageQueryRepository.GetMessage(request.MessageId, cancellationToken))
            ?? throw new MessageNotFoundException($"No Message found with id {request.MessageId}");

        message.MarkAsSeenBy(user);

        _messagePersistenceRepository.UpdateMessage(message);

        await _publisher.Publish(new MessageMarkedAsReadNotification(user.Id, message.Id), cancellationToken);
    }
}