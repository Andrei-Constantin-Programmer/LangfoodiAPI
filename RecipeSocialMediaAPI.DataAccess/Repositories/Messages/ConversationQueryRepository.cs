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
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConversationQueryRepository : IConversationQueryRepository
{
    private readonly ILogger<ConversationQueryRepository> _logger;
    private readonly IConversationDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<ConversationDocument> _conversationCollection;
    private readonly IConnectionQueryRepository _connectionQueryRepository;
    private readonly IGroupQueryRepository _groupQueryRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;

    public ConversationQueryRepository(ILogger<ConversationQueryRepository> logger, IConversationDocumentToModelMapper conversationDocumentToModelMapper, IMongoCollectionFactory mongoCollectionFactory, IConnectionQueryRepository connectionQueryRepository, IGroupQueryRepository groupQueryRepository, IMessageQueryRepository messageQueryRepository)
    {
        _logger = logger;
        _mapper = conversationDocumentToModelMapper;
        _conversationCollection = mongoCollectionFactory.CreateCollection<ConversationDocument>();
        _connectionQueryRepository = connectionQueryRepository;
        _groupQueryRepository = groupQueryRepository;
        _messageQueryRepository = messageQueryRepository;
    }

    public Conversation? GetConversationById(string id)
    {
        ConversationDocument? conversationDocument;
        try
        {
            conversationDocument = _conversationCollection.Find(
                conversationDoc => (conversationDoc.Id == id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get conversation with id {id}: {ErrorMessage}", id, ex.Message);
            return null;
        }

        var connection = _connectionQueryRepository.GetConnection();

        return conversationDocument is not null
            ? _mapper.MapConversationFromDocument(conversationDocument)
            : null;
    }

    public List<Conversation> GetConversationsByUser(IUserAccount userAccount)
    {
        List<ConversationDocument> conversations = new();

        try
        {
            var groups = _groupQueryRepository.GetGroupsByUser(userAccount); // TODO: Implement condition for collecting conversations#
            var connections = _connectionQueryRepository.GetConnectionsForUser(userAccount);

            conversations = _conversationCollection
                .GetAll(conversationDoc => conversationDoc.ConnectionId == null 
                                        ? groups.Any(group => group.GroupId == conversationDoc.GroupId) 
                                        : connections.Any(connection => connection.ConnectionId == conversationDoc.ConnectionId));

            

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

    private List<Message> GetMessages(ConversationDocument conversationDocument) => conversationDocument.Messages
        .Select(_messageQueryRepository.GetMessage)
        .OfType<Message>()
        .ToList();
    private Group? GetGroup(ConversationDocument conversationDocument) => conversationDocument.GroupId is null
        ? null
        : _groupQueryRepository.GetGroupById(conversationDocument.GroupId);

    private IConnection? GetConnection(ConversationDocument conversationDocument) => conversationDocument.ConnectionId is null
        ? null
        : _connectionQueryRepository.GetConnection(conversationDocument.ConnectionId);
}
