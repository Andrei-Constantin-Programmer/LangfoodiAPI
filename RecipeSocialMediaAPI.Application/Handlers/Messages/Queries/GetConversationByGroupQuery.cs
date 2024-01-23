using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConversationByGroupQuery(string GroupId) : IRequest<GroupConversationDTO>;

internal class GetConversationByGroupHandler : IRequestHandler<GetConversationByGroupQuery, GroupConversationDTO>
{
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IMessageMapper _messageMapper;

    public GetConversationByGroupHandler(IConversationQueryRepository conversationQueryRepository, IMessageMapper messageMapper)
    {
        _conversationQueryRepository = conversationQueryRepository;
        _messageMapper = messageMapper;
    }

    public Task<GroupConversationDTO> Handle(GetConversationByGroupQuery request, CancellationToken cancellationToken)
    {
        var conversation = _conversationQueryRepository.GetConversationByGroup(request.GroupId)
            ?? throw new ConversationNotFoundException($"No Conversation found for Connection with id {request.GroupId}");

        var lastMessage = conversation.Messages
            .MaxBy(message => message.SentDate);

        var lastMessageDto = lastMessage is null ? null : _messageMapper.MapMessageToMessageDTO(lastMessage);

        return Task.FromResult(new GroupConversationDTO(conversation.ConversationId, request.GroupId, lastMessageDto));
    }
}
