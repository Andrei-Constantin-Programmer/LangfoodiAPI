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
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record SendMessageCommand(NewMessageContract Contract) : IValidatableRequest<MessageDTO>;

internal class SendMessageHandler : IRequestHandler<SendMessageCommand, MessageDTO>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;
    private readonly IRecipeQueryRepository _recipeQueryRepository;
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IMessageMapper _messageMapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SendMessageHandler(
        IMessagePersistenceRepository messagePersistenceRepository,
        IUserQueryRepository userQueryRepository,
        IMessageQueryRepository messageQueryRepository,
        IRecipeQueryRepository recipeQueryRepository,
        IConversationQueryRepository conversationQueryRepository,
        IMessageMapper messageMapper,
        IDateTimeProvider dateTimeProvider)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _userQueryRepository = userQueryRepository;
        _messageQueryRepository = messageQueryRepository;
        _recipeQueryRepository = recipeQueryRepository;
        _conversationQueryRepository = conversationQueryRepository;
        _dateTimeProvider = dateTimeProvider;
        _messageMapper = messageMapper;
    }

    public async Task<MessageDTO> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        IUserAccount sender = _userQueryRepository.GetUserById(request.Contract.SenderId)?.Account
            ?? throw new UserNotFoundException($"User with id {request.Contract.SenderId} not found");

        if (_conversationQueryRepository.GetConversationById(request.Contract.ConversationId) is null)
        {
            throw new ConversationNotFoundException($"Conversation with id {request.Contract.ConversationId} was not found");
        }

        foreach (var recipeId in request.Contract.RecipeIds)
        {
            if(_recipeQueryRepository.GetRecipeById(recipeId) is null)
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
            text: request.Contract.Text.Trim(),
            recipeIds: request.Contract.RecipeIds,
            imageURLs: request.Contract.ImageURLs,
            sentDate: _dateTimeProvider.Now,
            messageRepliedTo: messageRepliedTo
        );

        return await Task.FromResult(_messageMapper.MapMessageToMessageDTO(createdMessage));
    }
}

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Contract.Text)
            .Must((command, text) => !string.IsNullOrWhiteSpace(text) || command.Contract.ImageURLs.Any() || command.Contract.RecipeIds.Any())
            .WithMessage("Message content must not be empty");

        RuleFor(x => x.Contract)
            .Must((_, contract) => contract.ImageURLs.Count > 0 && contract.RecipeIds.Count > 0)
            .WithMessage("A message cannot contain both images and recipes");
    }
}
