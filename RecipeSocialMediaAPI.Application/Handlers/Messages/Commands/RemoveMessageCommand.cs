using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record RemoveMessageCommand(string Id) : IRequest;

internal class RemoveMessageHandler : IRequestHandler<RemoveMessageCommand>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;
    private readonly IPublisher _publisher;

    public RemoveMessageHandler(IMessagePersistenceRepository messagePersistenceRepository, IMessageQueryRepository messageQueryRepository, IPublisher publisher)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageQueryRepository = messageQueryRepository;
        _publisher = publisher;
    }

    public async Task Handle(RemoveMessageCommand request, CancellationToken cancellationToken)
    {
        if (_messageQueryRepository.GetMessage(request.Id) is null)
        {
            throw new MessageNotFoundException(request.Id);
        }

        bool isSuccessful = _messagePersistenceRepository.DeleteMessage(request.Id);

        if (!isSuccessful)
        {
            throw new MessageRemovalException(request.Id);
        }

        await _publisher.Publish(new MessageDeletedNotification(request.Id), cancellationToken);
    }
}