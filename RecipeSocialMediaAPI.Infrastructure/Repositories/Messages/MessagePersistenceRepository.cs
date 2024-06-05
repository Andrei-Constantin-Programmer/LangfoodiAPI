using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;

public class MessagePersistenceRepository : IMessagePersistenceRepository
{
    private readonly ILogger<MessagePersistenceRepository> _logger;
    private readonly IMessageDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<MessageDocument> _messageCollection;
    private readonly IDataCryptoService _dataCryptoService;

    public MessagePersistenceRepository(
        ILogger<MessagePersistenceRepository> logger,
        IMessageDocumentToModelMapper mapper,
        IMongoCollectionFactory mongoCollectionFactory,
        IDataCryptoService dataCryptoService)
    {
        _logger = logger;
        _mapper = mapper;
        _messageCollection = mongoCollectionFactory.CreateCollection<MessageDocument>();
        _dataCryptoService = dataCryptoService;
    }

    public async Task<Message> CreateMessageAsync(IUserAccount sender, string? text, List<string>? recipeIds, List<string>? imageURLs, DateTimeOffset sentDate, Message? messageRepliedTo, List<string> seenByUserIds, CancellationToken cancellationToken = default)
    {
        MessageDocument messageDocument = await _messageCollection.InsertAsync(new MessageDocument(
            SenderId: sender.Id,
            MessageContent: new(EncryptTextMessage(text), recipeIds, imageURLs),
            SeenByUserIds: seenByUserIds,
            SentDate: sentDate,
            MessageRepliedToId: messageRepliedTo?.Id
        ), cancellationToken);

        return await _mapper.MapMessageFromDocumentAsync(messageDocument, sender, messageRepliedTo, cancellationToken);
    }

    public async Task<bool> UpdateMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _messageCollection.UpdateAsync(new MessageDocument(
                Id: message.Id,
                SenderId: message.Sender.Id,
                MessageContent: message switch
                {
                    TextMessage textMessage => new(EncryptTextMessage(textMessage.TextContent), null, null),
                    ImageMessage imageMessage => new(EncryptTextMessage(imageMessage.TextContent), null, imageMessage.ImageURLs.ToList()),
                    RecipeMessage recipeMessage => new(EncryptTextMessage(recipeMessage.TextContent), recipeMessage.Recipes.Select(recipe => recipe.Id).ToList(), null),

                    _ => throw new Exception($"Unable to update message with id {message.Id}")
                },
                SeenByUserIds: message.GetSeenBy().Select(user => user.Id).ToList(),
                SentDate: message.SentDate,
                LastUpdatedDate: message.UpdatedDate,
                MessageRepliedToId: message.RepliedToMessage?.Id
            ),
            doc => doc.Id == message.Id,
            cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was a problem updating message {MessageId}: {ErrorMessage}", message.Id, ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteMessageAsync(Message message, CancellationToken cancellationToken = default)
        => await DeleteMessageAsync(message.Id, cancellationToken);

    public async Task<bool> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default)
        => await _messageCollection.DeleteAsync(messageDoc => messageDoc.Id == messageId, cancellationToken);

    private string? EncryptTextMessage(string? textMessage) =>
        textMessage is null ? null : _dataCryptoService.Encrypt(textMessage);
}
