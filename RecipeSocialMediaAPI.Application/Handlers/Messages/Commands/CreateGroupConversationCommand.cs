using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateGroupConversationCommand(NewConversationContract Contract) : IRequest<GroupConversationDTO>;

internal class CreateGroupConversationHandler : IRequestHandler<CreateGroupConversationCommand, GroupConversationDTO>
{
    private readonly IConversationPersistenceRepository _conversationPersistenceRepository;
    private readonly IGroupQueryRepository _groupQueryRepository;

    public CreateGroupConversationHandler(IConversationPersistenceRepository conversationPersistenceRepository, IGroupQueryRepository groupQueryRepository)
    {
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _groupQueryRepository = groupQueryRepository;
    }

    public async Task<GroupConversationDTO> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
    {
        Group group = _groupQueryRepository.GetGroupById(request.Contract.GroupOrConnectionId)
                                    ?? throw new GroupNotFoundException(request.Contract.GroupOrConnectionId);

        Conversation newConversation = _conversationPersistenceRepository.CreateGroupConversation(group);

        return await Task.FromResult(new GroupConversationDTO(
            newConversation.ConversationId,
            request.Contract.GroupOrConnectionId,
            null
        ));
    }
}
