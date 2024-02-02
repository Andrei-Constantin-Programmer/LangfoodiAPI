using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateConnectionConversationCommand(NewConversationContract Contract) : IRequest<ConnectionConversationDTO>;

internal class CreateConnectionConversationHandler : IRequestHandler<CreateConnectionConversationCommand, ConnectionConversationDTO>
{
    private readonly IConversationPersistenceRepository _conversationPersistenceRepository;
    private readonly IConnectionQueryRepository _connectionQueryRepository;

    public CreateConnectionConversationHandler(IConversationPersistenceRepository conversationPersistenceRepository, IConnectionQueryRepository connectionQueryRepository)
    {
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _connectionQueryRepository = connectionQueryRepository;
    }

    public async Task<ConnectionConversationDTO> Handle(CreateConnectionConversationCommand request, CancellationToken cancellationToken)
    {
        IConnection connection = _connectionQueryRepository.GetConnection(request.Contract.GroupOrConnectionId)
                                    ?? throw new ConnectionNotFoundException($"Connection with id {request.Contract.GroupOrConnectionId} was not found");

        Conversation newConversation = _conversationPersistenceRepository.CreateConnectionConversation(connection);

        return await Task.FromResult(new ConnectionConversationDTO(
            newConversation.ConversationId,
            request.Contract.GroupOrConnectionId,
            null
        ));
    }
}
