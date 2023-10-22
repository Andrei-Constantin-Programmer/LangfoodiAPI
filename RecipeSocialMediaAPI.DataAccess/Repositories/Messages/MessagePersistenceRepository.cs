using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class MessagePersistenceRepository
{
    private readonly IMessageDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<MessageDocument> _messageCollection;
    private readonly IMessageFactory _messageFactory;

    public MessagePersistenceRepository(IMessageDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory, IMessageFactory messageFactory)
    {
        _mapper = mapper;
        _messageCollection = mongoCollectionFactory.CreateCollection<MessageDocument>();
    }

    public Message CreateMessage(IUserAccount sender, string? text, List<string> recipeIds, List<string> imageURLs, DateTimeOffset sentDate, Message messageRepliedTo)
    {
        MessageDocument messageDocument = _messageCollection.Insert(new MessageDocument()
        {
            SenderId = sender.Id,
            MessageContent = new(text, recipeIds, imageURLs),
            SentDate = sentDate,
            MessageRepliedToId = messageRepliedTo.Id
        });

        return _mapper.MapMessageFromDocument(messageDocument, sender, messageRepliedTo);
    }
}
