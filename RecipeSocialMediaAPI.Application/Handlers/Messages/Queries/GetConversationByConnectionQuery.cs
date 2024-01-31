using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConversationByConnectionQuery(string ConnectionId) : IRequest<ConnectionConversationDTO>;

internal class GetConversationByConnectionHandler : IRequestHandler<GetConversationByConnectionQuery, ConnectionConversationDTO>
{
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IMessageMapper _messageMapper;

    public GetConversationByConnectionHandler(IConversationQueryRepository conversationQueryRepository, IMessageMapper messageMapper)
    {
        _conversationQueryRepository = conversationQueryRepository;
        _messageMapper = messageMapper;
    }

    public Task<ConnectionConversationDTO> Handle(GetConversationByConnectionQuery request, CancellationToken cancellationToken)
    {
        var conversation = _conversationQueryRepository.GetConversationByConnection(request.ConnectionId)
            ?? throw new ConversationNotFoundException($"No Conversation found for Connection with id {request.ConnectionId}");

        var lastMessage = conversation.Messages
            .MaxBy(message => message.SentDate);

        var lastMessageDto = lastMessage is null ? null : _messageMapper.MapMessageToMessageDTO(lastMessage);

        return Task.FromResult(new ConnectionConversationDTO(conversation.ConversationId, request.ConnectionId, lastMessageDto));
    }
}
