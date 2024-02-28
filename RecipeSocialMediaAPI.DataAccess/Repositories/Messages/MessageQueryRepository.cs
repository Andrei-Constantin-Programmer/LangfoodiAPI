using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class MessageQueryRepository : IMessageQueryRepository
{
    private readonly ILogger<MessageQueryRepository> _logger;
    private readonly IMessageDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<MessageDocument> _messageCollection;
    private readonly IUserQueryRepository _userQueryRepository;

    public MessageQueryRepository(ILogger<MessageQueryRepository> logger, IMessageDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory, IUserQueryRepository userQueryRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _messageCollection = mongoCollectionFactory.CreateCollection<MessageDocument>();
        _userQueryRepository = userQueryRepository;
    }

    public Message? GetMessage(string id)
    {
        MessageDocument? messageDocument;
        try
        {
            messageDocument = _messageCollection.Find(messageDoc => messageDoc.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get message by id {MessageId}: {ErrorMessage}", id, ex.Message);
            messageDocument = null;
        }

        if (messageDocument is null)
        {
            return null;
        }

        IUserAccount? sender = GetSender(messageDocument.SenderId, messageDocument.Id);
        if (sender is null)
        {
            return null;
        }

        Message? repliedToMessage = GetRepliedToMessage(messageDocument);

        return _mapper.MapMessageFromDocument(messageDocument, sender, repliedToMessage);
    }

    public IEnumerable<Message> GetMessagesWithRecipe(string recipeId)
    {
        var messageDocuments = _messageCollection.GetAll(messageDoc 
            => messageDoc.MessageContent.RecipeIds != null 
            && messageDoc.MessageContent.RecipeIds.Contains(recipeId));
        
        return messageDocuments
            .Select(messageDoc => _mapper.MapMessageFromDocument(
                messageDoc, 
                GetSender(messageDoc.SenderId, messageDoc.Id) 
                    ?? throw new UserDocumentNotFoundException(messageDoc.SenderId), 
                GetRepliedToMessage(messageDoc)));
    }

    private IUserAccount? GetSender(string senderId, string? messageId)
    {
        IUserAccount? sender = _userQueryRepository.GetUserById(senderId)?.Account;
        if (sender is null)
        {
            _logger.LogWarning("The sender with id {SenderId} was not found for message with id {MessageId}", senderId, messageId);
        }

        return sender;
    }

    private Message? GetRepliedToMessage(MessageDocument messageDocument) => 
        messageDocument.MessageRepliedToId is not null
            ? GetMessage(messageDocument.MessageRepliedToId)
            : null;
}
