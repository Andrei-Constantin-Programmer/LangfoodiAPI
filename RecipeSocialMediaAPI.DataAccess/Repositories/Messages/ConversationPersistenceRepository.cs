using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
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

    public ConversationPersistenceRepository(ILogger<ConversationPersistenceRepository> logger, IConversationDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _logger = logger;
        _mapper = mapper;
        _conversationCollection = mongoCollectionFactory.CreateCollection<ConversationDocument>();
    }

    public Conversation CreateConnectionConversation(Connection connection)
    {
        throw new NotImplementedException();
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
