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
    private readonly IConversationMapper _conversationMapper;
    private readonly IUserQueryRepository _userQueryRepository;

    public GetConversationByGroupHandler(IConversationQueryRepository conversationQueryRepository, IConversationMapper conversationMapper, IUserQueryRepository userQueryRepository)
    {
        _conversationQueryRepository = conversationQueryRepository;
        _conversationMapper = conversationMapper;
        _userQueryRepository = userQueryRepository;
    }

    public Task<GroupConversationDTO> Handle(GetConversationByGroupQuery request, CancellationToken cancellationToken)
    {
        var user = _userQueryRepository.GetUserById(request.UserId)?.Account
            ?? throw new UserNotFoundException($"No User found with id {request.UserId}");

        var conversation = _conversationQueryRepository.GetConversationByGroup(request.GroupId)
            ?? throw new ConversationNotFoundException($"No Conversation found for Connection with id {request.GroupId}");

        return Task.FromResult(_conversationMapper.MapConversationToGroupConversationDTO(user, conversation));
    }
}
