using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class MessagePersistenceRepository
{
    private readonly IMessageDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<MessageDocument> _messageCollection;
    
    public MessagePersistenceRepository(IMessageDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory) 
    {
        _mapper = mapper;
        _messageCollection = mongoCollectionFactory.CreateCollection<MessageDocument>();
    }

    public Message CreateMessage(IUserAccount sender, string? text, List<string> recipeIds, List<string> imageURLs, DateTimeOffset sentDate, Message? messageRepliedTo)
    {
        MessageDocument messageDocument = _messageCollection.Insert(new MessageDocument()
        {
            SenderId = sender.Id,
            MessageContent = new(text, recipeIds, imageURLs),
            SentDate = sentDate,
            MessageRepliedToId = messageRepliedTo?.Id
        });

        return _mapper.MapMessageFromDocument(messageDocument, sender, messageRepliedTo);
    }
}
