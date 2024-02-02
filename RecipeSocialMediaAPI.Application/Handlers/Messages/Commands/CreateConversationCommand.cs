using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateConversationCommand(NewConversationContract Contract) : IRequest<ConnectionConversationDTO>;

internal class CreateConversationHandler : IRequestHandler<CreateConnectionCommand, ConnectionDTO>
{
    private readonly IConversationPersistenceRepository _conversationPersistenceRepository;
    private readonly IConnectionQueryRepository _connectionQueryRepository;
    private readonly IGroupQueryRepository _groupQueryRepository;

    public CreateConversationHandler(IConversationPersistenceRepository conversationPersistenceRepository, IConnectionQueryRepository connectionQueryRepository)
    {
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _connectionQueryRepository = connectionQueryRepository;
    }

    public async Task<> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {

        if(_connectionQueryRepository.GetConnection(request.Contract.GroupOrConnectionId) != null) 
        {
            Conversation newConversation = _conversationPersistenceRepository.CreateConnectionConversation(_connectionQueryRepository.GetConnection(request.Contract.GroupOrConnectionId));
            if (newConversation != null)
            {
                return await Task.FromResult(new ConnectionConversationDTO(
                    newConversation.ConversationId,
                    request.Contract.GroupOrConnectionId,
                    null
                ));
            }
        }

        else if(_groupQueryRepository.GetGroupById(request.Contract.GroupOrConnectionId) != null)
        {
            Conversation newConversation = _conversationPersistenceRepository.CreateGroupConversation(_groupQueryRepository.GetGroupById(request.Contract.GroupOrConnectionId));
            if (newConversation != null)
            {
                return await Task.FromResult(new GroupConversationDTO(
                    newConversation.ConversationId,
                    request.Contract.GroupOrConnectionId,
                    null
                ));
            }
        }

        throw new ArgumentException($"Given Id {request.Contract.GroupOrConnectionId} does not match to a Conversation or Group");
    }
}
