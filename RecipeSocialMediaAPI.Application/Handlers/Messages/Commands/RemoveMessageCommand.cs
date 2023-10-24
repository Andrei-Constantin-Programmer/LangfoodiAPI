using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record RemoveMessageCommand(string Id) : IRequest<bool>;

public class RemoveMessageHandler : IRequestHandler<RemoveMessageCommand, bool>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;

    public RemoveMessageHandler(IMessagePersistenceRepository messagePersistenceRepository, IMessageQueryRepository messageQueryRepository)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageQueryRepository = messageQueryRepository;
    }

    public Task<bool> Handle(RemoveMessageCommand request, CancellationToken cancellationToken) => throw new NotImplementedException();
}