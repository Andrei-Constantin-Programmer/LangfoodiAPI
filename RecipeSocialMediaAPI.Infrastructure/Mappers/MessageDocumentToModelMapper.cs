using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers;

public class MessageDocumentToModelMapper : IMessageDocumentToModelMapper
{
    private readonly ILogger<MessageDocumentToModelMapper> _logger;
    private readonly IMessageFactory _messageFactory;
    private readonly IRecipeQueryRepository _recipeQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IDataCryptoService _dataCryptoService;

    public MessageDocumentToModelMapper(
        ILogger<MessageDocumentToModelMapper> logger,
        IMessageFactory messageFactory,
        IRecipeQueryRepository recipeQueryRepository,
        IUserQueryRepository userQueryRepository,
        IDataCryptoService dataCryptoService)
    {
        _logger = logger;
        _messageFactory = messageFactory;
        _recipeQueryRepository = recipeQueryRepository;
        _userQueryRepository = userQueryRepository;
        _dataCryptoService = dataCryptoService;
    }

    public async Task<Message> MapMessageFromDocumentAsync(MessageDocument messageDocument, IUserAccount sender, Message? repliedToMessage, CancellationToken cancellationToken = default)
    {
        if (messageDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Message Document with null ID to Message");
        }

        var (text, recipeIds, imageURLs) = messageDocument.MessageContent;
        return (text, recipeIds, imageURLs) switch
        {
            (object, null, null) => await MapMessageDocumentToTextMessageAsync(messageDocument, sender, repliedToMessage, cancellationToken),
            (_, null, object) => await MapMessageDocumentToImageMessageAsync(messageDocument, sender, repliedToMessage, cancellationToken),
            (_, object, null) => await GetRecipeMessageAsync(repliedToMessage, cancellationToken),

            _ => throw new MalformedMessageDocumentException(messageDocument)
        };

        async Task<Message> GetRecipeMessageAsync(Message? repliedToMessage, CancellationToken cancellationToken = default)
        {
            var recipes = (await Task.WhenAll(recipeIds
                .Select(async id =>
                {
                    var recipe = await _recipeQueryRepository.GetRecipeByIdAsync(id, cancellationToken);
                    if (recipe is null)
                    {
                        _logger.LogWarning("No recipe with id {RecipeId} found for message with id {MessageId}", id, messageDocument.Id);
                    }

                    return recipe;
                })))
                .OfType<Recipe>();

            if (recipes.IsNullOrEmpty())
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new MalformedMessageDocumentException(messageDocument);
                }

                _logger.LogWarning("Malformed message found with no existing recipes: {MessageId}", messageDocument.Id);
                return await MapMessageDocumentToTextMessageAsync(messageDocument, sender, repliedToMessage, cancellationToken);
            }

            return await MapMessageDocumentToRecipeMessageAsync(messageDocument, sender, recipes, repliedToMessage, cancellationToken);
        }
    }

    private async Task<Message> MapMessageDocumentToTextMessageAsync(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null, CancellationToken cancellationToken = default)
    {
        return _messageFactory.CreateTextMessage(
            messageDocument.Id!,
            sender,
            DecryptTextMessage(messageDocument.MessageContent.Text) ?? throw new ArgumentException("Cannot map Text Message Document with null text"),
            await GetSeenByUsersAsync(messageDocument, cancellationToken),
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    private async Task<Message> MapMessageDocumentToImageMessageAsync(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null, CancellationToken cancellationToken = default)
    {
        return _messageFactory.CreateImageMessage(
            messageDocument.Id!,
            sender,
            messageDocument.MessageContent.ImageURLs!,
            DecryptTextMessage(messageDocument.MessageContent.Text),
            await GetSeenByUsersAsync(messageDocument, cancellationToken),
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    private async Task<Message> MapMessageDocumentToRecipeMessageAsync(MessageDocument messageDocument, IUserAccount sender, IEnumerable<Recipe> recipes, Message? messageRepliedTo = null, CancellationToken cancellationToken = default)
    {
        return _messageFactory.CreateRecipeMessage(
            messageDocument.Id!,
            sender,
            recipes,
            DecryptTextMessage(messageDocument.MessageContent.Text),
            await GetSeenByUsersAsync(messageDocument, cancellationToken),
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    private async Task<List<IUserAccount>> GetSeenByUsersAsync(MessageDocument messageDocument, CancellationToken cancellationToken = default) 
        => (await Task.WhenAll(messageDocument.SeenByUserIds
            .Select(async userId => await _userQueryRepository.GetUserByIdAsync(userId, cancellationToken))))
            .Where(user => user is not null)
            .Select(user => user!.Account)
            .ToList();

    private string? DecryptTextMessage(string? textMessage) =>
        textMessage is null ? null : _dataCryptoService.Decrypt(textMessage);
}
