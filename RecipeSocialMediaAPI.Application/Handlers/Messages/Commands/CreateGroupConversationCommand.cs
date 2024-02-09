using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateGroupConversationCommand(string UserId, string GroupId) : IRequest<ConversationDTO>;

internal class CreateGroupConversationHandler : IRequestHandler<CreateGroupConversationCommand, ConversationDTO>
{
    private readonly IConversationPersistenceRepository _conversationPersistenceRepository;
    private readonly IGroupQueryRepository _groupQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public CreateGroupConversationHandler(IConversationPersistenceRepository conversationPersistenceRepository, IGroupQueryRepository groupQueryRepository, IUserQueryRepository userQueryRepository)
    {
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _groupQueryRepository = groupQueryRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<ConversationDTO> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
    {
        _ = _userQueryRepository.GetUserById(request.UserId)?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.UserId}");

        Group group = _groupQueryRepository.GetGroupById(request.GroupId)
                                    ?? throw new GroupNotFoundException(request.GroupId);

        Conversation newConversation = _conversationPersistenceRepository.CreateGroupConversation(group);

        return await Task.FromResult(new ConversationDTO(
            newConversation.ConversationId,
            request.GroupId,
            true,
            group.GroupName,
            null,
            null
        ));
    }
}
