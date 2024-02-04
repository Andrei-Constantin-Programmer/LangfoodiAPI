using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record SendMessageCommand(NewMessageContract Contract) : IRequest<MessageDTO>;

internal class SendMessageHandler : IRequestHandler<SendMessageCommand, MessageDTO>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;
    private readonly IRecipeQueryRepository _recipeQueryRepository;
    private readonly IRecipePersistenceRepository _recipePersistenceRepository;
    private readonly IConversationQueryRepository _conversationQueryRepository;
    private readonly IConversationPersistenceRepository _conversationPersistenceRepository;
    private readonly IConnectionPersistenceRepository _connectionPersistenceRepository;
    public SendMessageHandler(IMessagePersistenceRepository messagePersistenceRepository, IUserQueryRepository userQueryRepository, IMessageQueryRepository messageQueryRepository, IRecipeQueryRepository recipeQueryRepository, IRecipePersistenceRepository recipePersistenceRepository, IConversationQueryRepository conversationQueryRepository, IConversationPersistenceRepository conversationPersistenceRepository, IConnectionPersistenceRepository connectionPersistenceRepository)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _userQueryRepository = userQueryRepository;
        _messageQueryRepository = messageQueryRepository;
        _recipeQueryRepository = recipeQueryRepository;
        _recipePersistenceRepository = recipePersistenceRepository;
        _conversationQueryRepository = conversationQueryRepository;
        _conversationPersistenceRepository = conversationPersistenceRepository;
        _connectionPersistenceRepository = connectionPersistenceRepository;
    }

    public async Task<MessageDTO> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        string text = request.Contract.Text.Trim();
        List<string>? imageURLs = request.Contract.ImageURLs;
        List<string>? recipeIds = request.Contract.RecipeIds;

        // Check if message content is empty
        if (text == "" && imageURLs.Count == 0 && recipeIds.Count == 0)
        {
            throw new CorruptedMessageException("Message content is empty");
        }

        // Create new connection and conversation if conversationId is null
        IUserAccount sender = _userQueryRepository.GetUserById(request.Contract.SenderId)?.Account
                    ?? throw new UserNotFoundException($"User with id {request.Contract.SenderId} not found");

        string? coversationId = request.Contract.ConversationId;

        if (coversationId == null)
        {
            if(request.Contract.RecipientId != null)
            {
                IUserAccount recipient = _userQueryRepository.GetUserById(request.Contract.RecipientId)?.Account
                            ?? throw new UserNotFoundException($"User with id {request.Contract.SenderId} not found");

                var connection = _connectionPersistenceRepository.CreateConnection(sender, recipient, ConnectionStatus.Pending);
                coversationId = _conversationPersistenceRepository.CreateConnectionConversation(connection).ConversationId;
            }
            else
            {
                throw new CorruptedMessageException("New connection can't be created as RecipientId is Null");
            }
        }

        // throw exception is conversationId is invalid
        else if (_conversationQueryRepository.GetConversationById(coversationId) == null)
        {
            throw new ConversationNotFoundException($"Conversation with id {request.Contract.ConversationId} was not found");
        }

        // Throw exception is a recipeId is invalid
        foreach (string recipeId in recipeIds)
        {
            if(_recipeQueryRepository.GetRecipeById(recipeId) == null)
            {
                throw new CorruptedMessageException($"recipe with id {recipeId} does not exist");
            }
        }

        // If given datetime string is invalid, use current datetime
        if (!DateTimeOffset.TryParse(request.Contract.SentDate, out DateTimeOffset sentDate))
        {
            sentDate = DateTimeOffset.UtcNow;
        }

        // get messageRepliedTo
        var messageRepliedTo = request.Contract.MessageRepliedTo == null ? null : _messageQueryRepository.GetMessage(request.Contract.MessageRepliedTo)?? throw new MessageNotFoundException(request.Contract.MessageRepliedTo);

        Message createdMessage = _messagePersistenceRepository.CreateMessage(
            sender: sender,
            text: text,
            recipeIds: recipeIds,
            imageURLs: imageURLs,
            sentDate: sentDate,
            messageRepliedTo: messageRepliedTo
            );

        // If imageURLs is empty, make it null for when MessageDTO is being created
        if (imageURLs.Count == 0)
        {
            imageURLs = null;
        }

        List<RecipePreviewDTO>? recipes = new();
        // If recipeIds is empty, make it null for when MessageDTO is being created
        if (recipeIds.Count == 0)
        {
            recipes = null;
        }
        // otherwise, create list of RecipePreviewDTOs
        else
        { 
            foreach(string recipeId in recipeIds)
            {
                RecipeAggregate recipeAggregate = _recipeQueryRepository.GetRecipeById(recipeId)!;

                recipes.Add(new(
                    Id: recipeAggregate.Id,
                    Title: recipeAggregate.Title,
                    ThumbnailId: recipeAggregate.ThumbnailId
                    ));
            }
        }

        return await Task.FromResult(new MessageDTO(
            Id: coversationId + "_" + createdMessage.Id,
            SenderId: createdMessage.Sender.Id,
            SenderName: createdMessage.Sender.UserName,
            SentDate: createdMessage.SentDate,
            UpdatedDate: null,
            RepliedToMessageId: createdMessage.RepliedToMessage!.Id,
            TextContent: text,
            ImageURLs: imageURLs,
            Recipes: recipes
            ));
    }
}
