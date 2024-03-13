using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record SendMessageCommand(SendMessageContract Contract) : IValidatableRequest<MessageDTO>;

internal class SendMessageHandler : IRequestHandler<SendMessageCommand, MessageDTO>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;
    private readonly IMessageMapper _messageMapper;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IConversationPersistenceRepository _conversationPersistenceRepository;
    private readonly IRecipeQueryRepository _recipeQueryRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPublisher _publisher;

    public SendMessageHandler(
        IMessagePersistenceRepository messagePersistenceRepository,
        IMessageQueryRepository messageQueryRepository,
        IMessageMapper messageMapper,
        IUserQueryRepository userQueryRepository,
        IConversationQueryRepository conversationQueryRepository,
        IConversationPersistenceRepository conversationPersistenceRepository,
        IRecipeQueryRepository recipeQueryRepository,
        IDateTimeProvider dateTimeProvider,
        IPublisher publisher)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageQueryRepository = messageQueryRepository;
        _messageMapper = messageMapper;
        _userQueryRepository = userQueryRepository;
        _conversationQueryRepository = conversationQueryRepository;
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _recipeQueryRepository = recipeQueryRepository;
        _dateTimeProvider = dateTimeProvider;
        _publisher = publisher;
    }

    public async Task<MessageDTO> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        IUserAccount sender = (await _userQueryRepository.GetUserByIdAsync(request.Contract.SenderId, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"User with id {request.Contract.SenderId} not found");

        Conversation conversation = (await _conversationQueryRepository.GetConversationByIdAsync(request.Contract.ConversationId, cancellationToken))
            ?? throw new ConversationNotFoundException($"Conversation with id {request.Contract.ConversationId} was not found");

        if (IsBlockedConnectionConversation(conversation, out var connectionId))
        {
            throw new AttemptedToSendMessageToBlockedConnectionException($"Cannot send message to connection {connectionId}, as it is blocked");
        }

        await ThrowIfRecipesNotFound(request.Contract.RecipeIds ?? new List<string>(), cancellationToken);
        Message? messageRepliedTo = await GetMessageRepliedTo(request, cancellationToken);

        Message createdMessage = await _messagePersistenceRepository.CreateMessageAsync(
            sender: sender,
            text: request.Contract.Text?.Trim(),
            recipeIds: request.Contract.RecipeIds,
            imageURLs: request.Contract.ImageURLs,
            sentDate: _dateTimeProvider.Now,
            messageRepliedTo: messageRepliedTo,
            seenByUserIds: new() { sender.Id },
            cancellationToken: cancellationToken
        );
        await SendMessageToConversation(conversation, createdMessage, cancellationToken);

        var messageDto = _messageMapper.MapMessageToMessageDTO(createdMessage);
        await _publisher.Publish(new MessageSentNotification(messageDto, conversation.ConversationId), cancellationToken);

        return messageDto;
    }

    private async Task ThrowIfRecipesNotFound(List<string> recipeIds, CancellationToken cancellationToken)
    {
        foreach (var recipeId in recipeIds)
        {
            if ((await _recipeQueryRepository.GetRecipeByIdAsync(recipeId, cancellationToken)) is null)
            {
                throw new RecipeNotFoundException(recipeId);
            }
        }
    }

    private async Task<Message?> GetMessageRepliedTo(SendMessageCommand request, CancellationToken cancellationToken) 
        => request.Contract.MessageRepliedToId is null
            ? null
            : await _messageQueryRepository.GetMessageAsync(request.Contract.MessageRepliedToId, cancellationToken)
                ?? throw new MessageNotFoundException(request.Contract.MessageRepliedToId);

    private async Task SendMessageToConversation(Conversation conversation, Message createdMessage, CancellationToken cancellationToken)
    {
        conversation.SendMessage(createdMessage);

        var (connection, group) = conversation switch
        {
            ConnectionConversation connConvo => (connConvo.Connection, (Group?)null),
            GroupConversation groupConvo => (null, groupConvo.Group),

            _ => throw new UnsupportedConversationException(conversation)
        };

        await _conversationPersistenceRepository.UpdateConversationAsync(conversation, connection, group, cancellationToken);
    }

    private static bool IsBlockedConnectionConversation(Conversation conversation, out string? connectionId)
    {
        if (conversation is not ConnectionConversation connConversation)
        {
            connectionId = null;
            return false;
        }

        connectionId = conversation.ConversationId;
        return connConversation.Connection.Account1.BlockedConnectionIds.Contains(connConversation.Connection.ConnectionId)
            || connConversation.Connection.Account2.BlockedConnectionIds.Contains(connConversation.Connection.ConnectionId);
    }
}

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Contract)
            .Must((_, contract) => !string.IsNullOrWhiteSpace(contract.Text) || (contract.ImageURLs?.Any() ?? false) || (contract.RecipeIds?.Any() ?? false))
            .WithMessage("Message content must not be empty");

        RuleFor(x => x.Contract)
            .Must((_, contract) => !(contract.ImageURLs?.Count > 0 && contract.RecipeIds?.Count > 0))
            .WithMessage("A message cannot contain both images and recipes");
    }
}
