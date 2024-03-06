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

    public async Task<Message> MapMessageFromDocument(MessageDocument messageDocument, IUserAccount sender, Message? repliedToMessage, CancellationToken cancellationToken = default)
    {
        var (text, recipeIds, imageURLs) = messageDocument.MessageContent;
        return (text, recipeIds, imageURLs) switch
        {
            (object, null, null) => await MapMessageDocumentToTextMessage(messageDocument, sender, repliedToMessage, cancellationToken),
            (_, null, object) => await MapMessageDocumentToImageMessage(messageDocument, sender, repliedToMessage, cancellationToken),
            (_, object, null) => await GetRecipeMessage(repliedToMessage, cancellationToken),

            _ => throw new MalformedMessageDocumentException(messageDocument)
        };

        async Task<Message> GetRecipeMessage(Message? repliedToMessage, CancellationToken cancellationToken = default)
        {
            var recipes = (await Task.WhenAll(recipeIds
                .Select(async id =>
                {
                    var recipe = await _recipeQueryRepository.GetRecipeById(id, cancellationToken);
                    if (recipe is null)
                    {
                        _logger.LogWarning("No recipe with id {RecipeId} found for message with id {MessageId}", id, messageDocument.Id);
                    }

                    return recipe;
                })))
                .OfType<RecipeAggregate>();

            if (recipes.IsNullOrEmpty())
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new MalformedMessageDocumentException(messageDocument);
                }

                _logger.LogWarning("Malformed message found with no existing recipes: {MessageId}", messageDocument.Id);
                return await MapMessageDocumentToTextMessage(messageDocument, sender, repliedToMessage, cancellationToken);
            }

            return await MapMessageDocumentToRecipeMessage(messageDocument, sender, recipes, repliedToMessage, cancellationToken);
        }
    }

    private async Task<Message> MapMessageDocumentToTextMessage(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null, CancellationToken cancellationToken = default)
    {
        if (messageDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Message Document with null ID to Message");
        }

        return _messageFactory.CreateTextMessage(
            messageDocument.Id,
            sender,
            messageDocument.MessageContent.Text!,
            await GetSeenByUsers(messageDocument, cancellationToken),
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    private async Task<Message> MapMessageDocumentToImageMessage(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null, CancellationToken cancellationToken = default)
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
            await GetSeenByUsers(messageDocument, cancellationToken),
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    private async Task<Message> MapMessageDocumentToRecipeMessage(MessageDocument messageDocument, IUserAccount sender, IEnumerable<RecipeAggregate> recipes, Message? messageRepliedTo = null, CancellationToken cancellationToken = default)
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
            await GetSeenByUsers(messageDocument, cancellationToken),
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    private async Task<List<IUserAccount>> GetSeenByUsers(MessageDocument messageDocument, CancellationToken cancellationToken = default) 
        => (await Task.WhenAll(messageDocument.SeenByUserIds
            .Select(async userId => await _userQueryRepository.GetUserById(userId, cancellationToken))))
            .Where(user => user is not null)
            .Select(user => user!.Account)
            .ToList();
}
