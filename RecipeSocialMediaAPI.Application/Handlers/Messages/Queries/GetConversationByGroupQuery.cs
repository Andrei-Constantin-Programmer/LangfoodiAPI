using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConversationByGroupQuery(string UserId, string GroupId) : IRequest<GroupConversationDTO>;

internal class GetConversationByGroupHandler : IRequestHandler<GetConversationByGroupQuery, GroupConversationDTO>
{
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IMessageMapper _messageMapper;
    private readonly IUserQueryRepository _userQueryRepository;

    public GetConversationByGroupHandler(IConversationQueryRepository conversationQueryRepository, IMessageMapper messageMapper, IUserQueryRepository userQueryRepository)
    {
        _conversationQueryRepository = conversationQueryRepository;
        _messageMapper = messageMapper;
        _userQueryRepository = userQueryRepository;
    }

    public Task<GroupConversationDTO> Handle(GetConversationByGroupQuery request, CancellationToken cancellationToken)
    {
        var user = _userQueryRepository.GetUserById(request.UserId)
            ?? throw new UserNotFoundException($"No User found with id {request.UserId}");

        var conversation = _conversationQueryRepository.GetConversationByGroup(request.GroupId)
            ?? throw new ConversationNotFoundException($"No Conversation found for Connection with id {request.GroupId}");

        var lastMessage = conversation.Messages
            .MaxBy(message => message.SentDate);

        var lastMessageDto = lastMessage is null ? null : _messageMapper.MapMessageToMessageDTO(lastMessage);
        var unreadCount = conversation.Messages
            .Count(message => message.SeenBy.Any(u => u.Id == user.Account.Id));

        return Task.FromResult(new GroupConversationDTO(conversation.ConversationId, request.GroupId, lastMessageDto, unreadCount));
    }
}
