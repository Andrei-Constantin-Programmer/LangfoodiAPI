using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConversationPersistenceRepository : IConversationPersistenceRepository
{
    private readonly ILogger<ConversationPersistenceRepository> _logger;
    private readonly IConversationDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<ConversationDocument> _conversationCollection;
    private readonly IMongoCollectionWrapper<ConnectionDocument> _connectionCollection;

    public ConversationPersistenceRepository(ILogger<ConversationPersistenceRepository> logger, IConversationDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _logger = logger;
        _mapper = mapper;
        _conversationCollection = mongoCollectionFactory.CreateCollection<ConversationDocument>();
        _connectionCollection = mongoCollectionFactory.CreateCollection<ConnectionDocument>();
    }

    public Conversation CreateConnectionConversation(IConnection connection)
    {
        ConnectionDocument connectionDocument = _connectionCollection.Find(conn 
                => (conn.AccountId1 == connection.Account1.Id && conn.AccountId2 == connection.Account2.Id)
                || (conn.AccountId1 == connection.Account2.Id && conn.AccountId2 == connection.Account1.Id))
            ?? throw new ConnectionDocumentNotFoundException(connection.Account1, connection.Account2);

        ConversationDocument conversationDocument = _conversationCollection.Insert(new()
        {
            ConnectionId = connectionDocument.Id,
            Messages = new()
        });

        return _mapper.MapConversationFromDocument(conversationDocument, connection, null, new());
    }

    public Conversation CreateGroupConversation(Group group)
    {
        throw new NotImplementedException();
    }

    public Conversation UpdateConversation(Conversation conversation)
    {
        throw new NotImplementedException();
    }

    public bool DeleteConversation(Conversation conversation)
    {
        throw new NotImplementedException();
    }

    public bool DeleteConversation(string conversationId)
    {
        throw new NotImplementedException();
    }
}
