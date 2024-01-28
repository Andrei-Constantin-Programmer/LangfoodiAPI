﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateConversationCommand(NewConversationContract Contract) : IRequest<ConnectionConversationDTO>;

internal class CreateConversationHandler : IRequestHandler<CreateConnectionCommand, ConnectionDTO>
{
    private readonly IConversationPersistenceRepository _conversationPersistenceRepository;
    private readonly IConnectionQueryRepository _connectionQueryRepository;

    public CreateConversationHandler(IConversationPersistenceRepository conversationPersistenceRepository, IConnectionQueryRepository connectionQueryRepository)
    {
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _connectionQueryRepository = connectionQueryRepository;
    }

}
