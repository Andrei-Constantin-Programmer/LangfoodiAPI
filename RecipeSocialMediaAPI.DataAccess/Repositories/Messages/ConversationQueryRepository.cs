using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConversationQueryRepository : IConversationQueryRepository
{
    private readonly ILogger<ConversationQueryRepository> _logger;
    private readonly IConversationDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<ConversationDocument> _conversationCollection;
    private readonly IConnectionQueryRepository _connectionQueryRepository;

    public ConversationQueryRepository(ILogger<ConversationQueryRepository> logger, IConversationDocumentToModelMapper conversationDocumentToModelMapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _logger = logger;
        _mapper = conversationDocumentToModelMapper;
        _conversationCollection = mongoCollectionFactory.CreateCollection<ConversationDocument>();
    }

    public Conversation? GetConversationById(string id)
    {
        ConversationDocument? conversationDocument;
        try
        {
            conversationDocument = _conversationCollection.Find(
                conversationDoc => (conversationDoc.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get conversation with id {id}: {ErrorMessage}", id, ex.Message);
            return null;
        }

        return conversationDocument is not null
            ? _mapper.MapConversationFromDocument(conversationDocument)
            : null;
    }

    public List<Conversation> GetConversationsByUser(IUserAccount userAccount)
    {
        List<ConversationDocument> conversations = new();

        try
        {
            conversations = _conversationCollection
                .GetAll(conversationDoc => ConversationContainsUser(userAccount, conversationDoc)); // TODO: Implement condition for collecting conversations#

            // need connection query repo and group query repo
            // connection and group contains users in different formats
            // check if user is in these list of users
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get the conversations for user with id {UserId}: {ErrorMessage}", userAccount.Id, ex.Message);
        }

        return conversations
            .Select(_mapper.MapConversationFromDocument)
            .ToList();
    }

    private bool ConversationContainsUser(IUserAccount userAccount, ConversationDocument conversationDocument)
    {

        throw new NotImplementedException();
    }
}
