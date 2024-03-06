﻿using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class MessagePersistenceRepository : IMessagePersistenceRepository
{
    private readonly ILogger<MessagePersistenceRepository> _logger;
    private readonly IMessageDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<MessageDocument> _messageCollection;

    public MessagePersistenceRepository(ILogger<MessagePersistenceRepository> logger, IMessageDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _logger = logger;
        _mapper = mapper;
        _messageCollection = mongoCollectionFactory.CreateCollection<MessageDocument>();
    }

    public async Task<Message> CreateMessage(IUserAccount sender, string? text, List<string>? recipeIds, List<string>? imageURLs, DateTimeOffset sentDate, Message? messageRepliedTo, List<string> seenByUserIds, CancellationToken cancellationToken = default)
    {
        MessageDocument messageDocument = await _messageCollection.Insert(new MessageDocument(
            SenderId: sender.Id,
            MessageContent: new(text, recipeIds, imageURLs),
            SeenByUserIds: seenByUserIds,
            SentDate: sentDate,
            MessageRepliedToId: messageRepliedTo?.Id
        ), cancellationToken);

        return await _mapper.MapMessageFromDocument(messageDocument, sender, messageRepliedTo, cancellationToken);
    }

    public async Task<bool> UpdateMessage(Message message, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _messageCollection.UpdateRecord(new MessageDocument(
                Id: message.Id,
                SenderId: message.Sender.Id,
                MessageContent: message switch
                {
                    TextMessage textMessage => new(textMessage.TextContent, null, null),
                    ImageMessage imageMessage => new(imageMessage.TextContent, null, imageMessage.ImageURLs.ToList()),
                    RecipeMessage recipeMessage => new(recipeMessage.TextContent, recipeMessage.Recipes.Select(recipe => recipe.Id).ToList(), null),

                    _ => throw new Exception($"Unable to update message with id {message.Id}")
                },
                SeenByUserIds: message.SeenBy.Select(user => user.Id).ToList(),
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

    public async Task<bool> DeleteMessage(Message message, CancellationToken cancellationToken = default) 
        => await DeleteMessage(message.Id, cancellationToken);

    public async Task<bool> DeleteMessage(string messageId, CancellationToken cancellationToken = default) 
        => await _messageCollection.Delete(messageDoc => messageDoc.Id == messageId, cancellationToken);
}
