﻿using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
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
    private readonly IConversationMapper _conversationMapper;
    private readonly IGroupQueryRepository _groupQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public CreateGroupConversationHandler(
        IConversationPersistenceRepository conversationPersistenceRepository,
        IConversationMapper conversationMapper,
        IGroupQueryRepository groupQueryRepository,
        IUserQueryRepository userQueryRepository)
    {
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _conversationMapper = conversationMapper;
        _groupQueryRepository = groupQueryRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<ConversationDTO> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
    {
        IUserAccount user = _userQueryRepository.GetUserById(request.UserId)?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.UserId}");

        Group group = _groupQueryRepository.GetGroupById(request.GroupId)
                                    ?? throw new GroupNotFoundException(request.GroupId);

        Conversation newConversation = _conversationPersistenceRepository.CreateGroupConversation(group);

        return await Task.FromResult(_conversationMapper.MapConversationToGroupConversationDTO(user, (GroupConversation)newConversation));
    }
}
