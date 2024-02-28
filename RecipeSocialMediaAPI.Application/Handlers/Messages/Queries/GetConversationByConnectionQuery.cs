using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConversationByConnectionQuery(string UserId, string ConnectionId) : IRequest<ConversationDTO>;

internal class GetConversationByConnectionHandler : IRequestHandler<GetConversationByConnectionQuery, ConversationDTO>
{
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IConversationMapper _conversationMapper;
    private readonly IUserQueryRepository _userQueryRepository;

    public GetConversationByConnectionHandler(IConversationQueryRepository conversationQueryRepository, IConversationMapper conversationMapper, IUserQueryRepository userQueryRepository)
    {
        _conversationQueryRepository = conversationQueryRepository;
        _conversationMapper = conversationMapper;
        _userQueryRepository = userQueryRepository;
    }

    public Task<ConversationDTO> Handle(GetConversationByConnectionQuery request, CancellationToken cancellationToken)
    {
        var user = _userQueryRepository.GetUserById(request.UserId)?.Account
            ?? throw new UserNotFoundException($"No User found with id {request.UserId}");

        var conversation = _conversationQueryRepository.GetConversationByConnection(request.ConnectionId)
            ?? throw new ConversationNotFoundException($"No Conversation found for Connection with id {request.ConnectionId}");

        return Task.FromResult(_conversationMapper.MapConversationToConnectionConversationDTO(user, conversation));
    }
}
