using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateConnectionConversationCommand(string UserId, string ConnectionId) : IRequest<ConversationDTO>;

internal class CreateConnectionConversationHandler : IRequestHandler<CreateConnectionConversationCommand, ConversationDTO>
{
    private readonly IConversationPersistenceRepository _conversationPersistenceRepository;
    private readonly IConnectionQueryRepository _connectionQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public CreateConnectionConversationHandler(IConversationPersistenceRepository conversationPersistenceRepository, IConnectionQueryRepository connectionQueryRepository, IUserQueryRepository userQueryRepository)
    {
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _connectionQueryRepository = connectionQueryRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<ConversationDTO> Handle(CreateConnectionConversationCommand request, CancellationToken cancellationToken)
    {
        IUserAccount user = _userQueryRepository.GetUserById(request.UserId)?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.UserId}");

        IConnection connection = _connectionQueryRepository.GetConnection(request.ConnectionId)
                                    ?? throw new ConnectionNotFoundException($"Connection with id {request.ConnectionId} was not found");

        Conversation newConversation = _conversationPersistenceRepository.CreateConnectionConversation(connection);

        return await Task.FromResult(new ConversationDTO(
            newConversation.ConversationId,
            request.ConnectionId,
            false,
            user.UserName,
            connection.Account1.Id == user.Id ? connection.Account2.ProfileImageId : connection.Account1.ProfileImageId,
            null
        ));
    }
}
