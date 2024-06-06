using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
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

    public async Task Handle(MarkConversationAsReadCommand request, CancellationToken cancellationToken)
    {
        IUserAccount user = (await _userQueryRepository.GetUserByIdAsync(request.UserId, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.UserId}");

        Conversation conversation = (await _conversationQueryRepository.GetConversationByIdAsync(request.ConversationId, cancellationToken))
            ?? throw new ConversationNotFoundException($"No conversation found with id {request.ConversationId}");

        var unseenMessages = conversation.GetMessages()
            .Where(message => !message.GetSeenBy().Exists(u => u.Id == user.Id))
            .ToList();

        foreach (var message in unseenMessages)
        {
            message.MarkAsSeenBy(user);
            await _messagePersistenceRepository.UpdateMessageAsync(message, cancellationToken);
        }
    }
}
