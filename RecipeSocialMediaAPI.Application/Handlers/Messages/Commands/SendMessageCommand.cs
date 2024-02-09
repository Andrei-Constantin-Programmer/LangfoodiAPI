using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
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

public record SendMessageCommand(NewMessageContract Contract) : IValidatableRequest<MessageDTO>;

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

    public SendMessageHandler(
        IMessagePersistenceRepository messagePersistenceRepository,
        IMessageQueryRepository messageQueryRepository,
        IMessageMapper messageMapper,
        IUserQueryRepository userQueryRepository,
        IConversationQueryRepository conversationQueryRepository,
        IConversationPersistenceRepository conversationPersistenceRepository,
        IRecipeQueryRepository recipeQueryRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageQueryRepository = messageQueryRepository;
        _messageMapper = messageMapper;
        _userQueryRepository = userQueryRepository;
        _conversationQueryRepository = conversationQueryRepository;
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _recipeQueryRepository = recipeQueryRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<MessageDTO> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        IUserAccount sender = _userQueryRepository.GetUserById(request.Contract.SenderId)?.Account
            ?? throw new UserNotFoundException($"User with id {request.Contract.SenderId} not found");

        Conversation conversation = _conversationQueryRepository.GetConversationById(request.Contract.ConversationId)
            ?? throw new ConversationNotFoundException($"Conversation with id {request.Contract.ConversationId} was not found");
        
        foreach (var recipeId in request.Contract.RecipeIds)
        {
            if (_recipeQueryRepository.GetRecipeById(recipeId) is null)
            {
                throw new RecipeNotFoundException(recipeId);
            }
        }

        Message? messageRepliedTo = request.Contract.MessageRepliedToId is null
            ? null
            : _messageQueryRepository.GetMessage(request.Contract.MessageRepliedToId)
                ?? throw new MessageNotFoundException(request.Contract.MessageRepliedToId);

        Message createdMessage = _messagePersistenceRepository.CreateMessage(
            sender: sender,
            text: request.Contract.Text?.Trim(),
            recipeIds: request.Contract.RecipeIds,
            imageURLs: request.Contract.ImageURLs,
            sentDate: _dateTimeProvider.Now,
            messageRepliedTo: messageRepliedTo,
            seenByUserIds: new() { sender.Id }
        );

        conversation.SendMessage(createdMessage);

        var (connection, group) = conversation switch
        {
            ConnectionConversation connConvo => (connConvo.Connection, (Group?)null),
            GroupConversation groupConvo => (null, groupConvo.Group),

            _ => throw new UnsupportedConversationException(conversation)
        };

        _conversationPersistenceRepository.UpdateConversation(conversation, connection, group);

        return await Task.FromResult(_messageMapper.MapMessageToMessageDTO(createdMessage));
    }
}

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Contract)
            .Must((_, contract) => !string.IsNullOrWhiteSpace(contract.Text) || contract.ImageURLs.Any() || contract.RecipeIds.Any())
            .WithMessage("Message content must not be empty");

        RuleFor(x => x.Contract)
            .Must((_, contract) => !(contract.ImageURLs.Count > 0 && contract.RecipeIds.Count > 0))
            .WithMessage("A message cannot contain both images and recipes");
    }
}
