using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record RemoveMessageCommand(string Id) : IRequest;

internal class RemoveMessageHandler : IRequestHandler<RemoveMessageCommand>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;

    public RemoveMessageHandler(IMessagePersistenceRepository messagePersistenceRepository, IMessageQueryRepository messageQueryRepository)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageQueryRepository = messageQueryRepository;
    }

    public Task Handle(RemoveMessageCommand request, CancellationToken cancellationToken)
    {
        if (_messageQueryRepository.GetMessage(request.Id) is null)
        {
            throw new MessageNotFoundException(request.Id);
        }

        bool isSuccessful = _messagePersistenceRepository.DeleteMessage(request.Id);

        return isSuccessful
            ? Task.CompletedTask
            : throw new MessageRemovalException(request.Id);
    }
}