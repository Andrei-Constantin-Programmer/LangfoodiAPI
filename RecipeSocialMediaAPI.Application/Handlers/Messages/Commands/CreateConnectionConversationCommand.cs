﻿using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
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
    private readonly IConversationMapper _conversationMapper;
    private readonly IConnectionQueryRepository _connectionQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public CreateConnectionConversationHandler(
        IConversationPersistenceRepository conversationPersistenceRepository,
        IConversationMapper conversationMapper,
        IConnectionQueryRepository connectionQueryRepository,
        IUserQueryRepository userQueryRepository)
    {
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _conversationMapper = conversationMapper;
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

        return await Task.FromResult(_conversationMapper.MapConversationToConnectionConversationDTO(user, (ConnectionConversation)newConversation));
    }
}
