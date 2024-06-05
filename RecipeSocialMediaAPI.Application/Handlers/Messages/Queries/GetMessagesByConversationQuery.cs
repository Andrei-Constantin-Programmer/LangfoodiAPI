using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetMessagesByConversationQuery(string ConversationId) : IRequest<List<MessageDTO>>;

internal class GetMessagesByConversationHandler : IRequestHandler<GetMessagesByConversationQuery, List<MessageDTO>>
{
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IMessageMapper _messageMapper;

    public GetMessagesByConversationHandler(IConversationQueryRepository conversationQueryRepository, IMessageMapper messageMapper)
    {
        _conversationQueryRepository = conversationQueryRepository;
        _messageMapper = messageMapper;
    }

    public async Task<List<MessageDTO>> Handle(GetMessagesByConversationQuery request, CancellationToken cancellationToken)
    {
        Conversation conversation = await _conversationQueryRepository.GetConversationByIdAsync(request.ConversationId, cancellationToken)
            ?? throw new ConversationNotFoundException($"No conversation found with id {request.ConversationId}");

        return conversation.GetMessages()
            .Select(_messageMapper.MapMessageToMessageDTO)
            .ToList();
    }
}
