using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConversationsByUserQuery(string UserId) : IRequest<List<ConversationDTO>>;

internal class GetConversationsByUserHandler : IRequestHandler<GetConversationsByUserQuery, List<ConversationDTO>>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IConversationMapper _conversationMapper;

    public GetConversationsByUserHandler(IUserQueryRepository userQueryRepository, IConversationQueryRepository conversationQueryRepository, IConversationMapper conversationMapper)
    {
        _conversationQueryRepository = conversationQueryRepository;
        _userQueryRepository = userQueryRepository;
        _conversationMapper = conversationMapper;
    }

    public async Task<List<ConversationDTO>> Handle(GetConversationsByUserQuery request, CancellationToken cancellationToken)
    {
        IUserAccount user = _userQueryRepository.GetUserById(request.UserId)?.Account
            ?? throw new UserNotFoundException($"No User with id {request.UserId} was found");

        return await Task.FromResult(_conversationQueryRepository
            .GetConversationsByUser(user)
            .Select(conversation => (ConversationDTO)(conversation switch
            {
                ConnectionConversation connConvo => _conversationMapper.MapConversationToConnectionConversationDTO(user, connConvo),
                GroupConversation groupConvo => _conversationMapper.MapConversationToGroupConversationDTO(user, groupConvo),

                _ => throw new UnsupportedConversationException(conversation)
            }))
            .ToList()
        );
    }
}
