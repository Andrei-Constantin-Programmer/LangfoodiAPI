using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record MarkConversationAsReadCommand(string UserId, string ConversationId) : IRequest;

internal class MarkConversationAsReadHandler : IRequestHandler<MarkConversationAsReadCommand>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;

    public MarkConversationAsReadHandler(IUserQueryRepository userQueryRepository, IConversationQueryRepository conversationQueryRepository, IMessagePersistenceRepository messagePersistenceRepository)
    {
        _userQueryRepository = userQueryRepository;
        _conversationQueryRepository = conversationQueryRepository;
        _messagePersistenceRepository = messagePersistenceRepository;
    }

    public Task Handle(MarkConversationAsReadCommand request, CancellationToken cancellationToken)
    {
        IUserAccount user = _userQueryRepository.GetUserById(request.UserId)?.Account;
        Conversation conversation = _conversationQueryRepository.GetConversationById(request.ConversationId);

        var unseenMessages = conversation.Messages
            .Where(message => !message.SeenBy.Any(u => u.Id == user.Id))
            .ToList();

        foreach (var message in unseenMessages)
        {
            message.MarkAsSeenBy(user);
            _messagePersistenceRepository.UpdateMessage(message);
        }

        return Task.CompletedTask;
    }
}
