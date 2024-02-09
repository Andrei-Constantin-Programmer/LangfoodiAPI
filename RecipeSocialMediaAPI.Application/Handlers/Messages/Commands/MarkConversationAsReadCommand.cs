using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record MarkConversationAsReadCommand(string UserId, string ConversationId) : IRequest;

internal class MarkConversationAsReadHandler : IRequestHandler<MarkConversationAsReadCommand>
{
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;

    public MarkConversationAsReadHandler(IConversationQueryRepository conversationQueryRepository, IMessagePersistenceRepository messagePersistenceRepository)
    {
        _conversationQueryRepository = conversationQueryRepository;
        _messagePersistenceRepository = messagePersistenceRepository;
    }

    public Task Handle(MarkConversationAsReadCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
