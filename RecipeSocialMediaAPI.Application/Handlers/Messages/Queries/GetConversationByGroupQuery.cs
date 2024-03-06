using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConversationByGroupQuery(string UserId, string GroupId) : IRequest<ConversationDTO>;

internal class GetConversationByGroupHandler : IRequestHandler<GetConversationByGroupQuery, ConversationDTO>
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

    public async Task<ConversationDTO> Handle(GetConversationByGroupQuery request, CancellationToken cancellationToken)
    {
        var user = (await _userQueryRepository.GetUserById(request.UserId, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No User found with id {request.UserId}");

        var conversation = await _conversationQueryRepository.GetConversationByGroup(request.GroupId, cancellationToken)
            ?? throw new ConversationNotFoundException($"No Conversation found for Connection with id {request.GroupId}");

        return _conversationMapper.MapConversationToGroupConversationDTO(user, conversation);
    }
}
