using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public class MessageDocumentToModelMapper : IMessageDocumentToModelMapper
{
    private readonly ILogger<MessageDocumentToModelMapper> _logger;
    private readonly IMessageFactory _messageFactory;
    private readonly IRecipeQueryRepository _recipeQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public MessageDocumentToModelMapper(ILogger<MessageDocumentToModelMapper> logger, IMessageFactory messageFactory, IRecipeQueryRepository recipeQueryRepository, IUserQueryRepository userQueryRepository)
    {
        _logger = logger;
        _messageFactory = messageFactory;
        _recipeQueryRepository = recipeQueryRepository;
        _userQueryRepository = userQueryRepository;
    }

    public Message MapMessageFromDocument(MessageDocument messageDocument, IUserAccount sender, Message? repliedToMessage)
    {
        var (text, recipeIds, imageURLs) = messageDocument.MessageContent;
        return (text, recipeIds, imageURLs) switch
        {
            (object, null, null) => MapMessageDocumentToTextMessage(messageDocument, sender, repliedToMessage),
            (_, null, object) => MapMessageDocumentToImageMessage(messageDocument, sender, repliedToMessage),
            (_, object, null) => GetRecipeMessage(repliedToMessage),

            _ => throw new MalformedMessageDocumentException(messageDocument)
        };

        Message GetRecipeMessage(Message? repliedToMessage)
        {
            var recipes = recipeIds
                .Select(id =>
                {
                    var recipe = _recipeQueryRepository.GetRecipeById(id);
                    if (recipe is null)
                    {
                        _logger.LogWarning("No recipe with id {RecipeId} found for message with id {MessageId}", id, messageDocument.Id);
                    }

                    return recipe;
                })
                .OfType<RecipeAggregate>();

            if (recipes.IsNullOrEmpty())
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new MalformedMessageDocumentException(messageDocument);
                }

                _logger.LogWarning("Malformed message found with no existing recipes: {MessageId}", messageDocument.Id);
                return MapMessageDocumentToTextMessage(messageDocument, sender, repliedToMessage);
            }

            return MapMessageDocumentToRecipeMessage(messageDocument, sender, recipes, repliedToMessage);
        }
    }

    private Message MapMessageDocumentToTextMessage(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null)
    {
        if (messageDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Message Document with null ID to Message");
        }

        return _messageFactory.CreateTextMessage(
            messageDocument.Id,
            sender,
            messageDocument.MessageContent.Text!,
            GetSeenByUsers(messageDocument),
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    private Message MapMessageDocumentToImageMessage(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null)
    {
        if (messageDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Message Document with null ID to Message");
        }

        return _messageFactory.CreateImageMessage(
            messageDocument.Id,
            sender,
            messageDocument.MessageContent.ImageURLs!,
            messageDocument.MessageContent.Text,
            GetSeenByUsers(messageDocument),
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    private Message MapMessageDocumentToRecipeMessage(MessageDocument messageDocument, IUserAccount sender, IEnumerable<RecipeAggregate> recipes, Message? messageRepliedTo = null)
    {
        if (messageDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Message Document with null ID to Message");
        }

        return _messageFactory.CreateRecipeMessage(
            messageDocument.Id,
            sender,
            recipes,
            messageDocument.MessageContent.Text,
            GetSeenByUsers(messageDocument),
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    private List<IUserAccount> GetSeenByUsers(MessageDocument messageDocument) => messageDocument.SeenByUserIds
        .Select(userId => _userQueryRepository.GetUserById(userId))
        .Where(user => user is not null)
        .Select(user => user!.Account)
        .ToList();
}
