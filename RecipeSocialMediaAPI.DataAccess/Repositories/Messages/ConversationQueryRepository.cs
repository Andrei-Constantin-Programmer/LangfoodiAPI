using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using Microsoft.Extensions.Logging;
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

    public async Task<Conversation?> GetConversationById(string id, CancellationToken cancellationToken = default)
    {
        ConversationDocument? conversationDocument;
        try
        {
            conversationDocument = await _conversationCollection.Find(
                conversationDoc => conversationDoc.Id == id, cancellationToken);

            if (conversationDocument is null)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get conversation with id {id}: {ErrorMessage}", id, ex.Message);
            return null;
        }

        IConnection? connection = await GetConnection(conversationDocument, cancellationToken);
        Group? group = await GetGroup(conversationDocument, cancellationToken);
        List<Message> messages = await GetMessages(conversationDocument, cancellationToken);

        return _mapper.MapConversationFromDocument(conversationDocument, connection, group, messages);
    }

    public async Task<ConnectionConversation?> GetConversationByConnection(string connectionId, CancellationToken cancellationToken = default)
    {
        ConversationDocument? conversationDocument;
        try
        {
            conversationDocument = await _conversationCollection.Find(
                conversationDoc => conversationDoc.ConnectionId == connectionId, cancellationToken);
            if (conversationDocument is null)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get conversation for connection with id {id}: {ErrorMessage}", connectionId, ex.Message);
            return null;
        }

        IConnection? connection = await GetConnection(conversationDocument, cancellationToken);
        List<Message> messages = await GetMessages(conversationDocument, cancellationToken);

        return conversationDocument is not null
            ? (ConnectionConversation)_mapper.MapConversationFromDocument(conversationDocument, connection, null, messages)
            : null;
    }

    public async Task<GroupConversation?> GetConversationByGroup(string groupId, CancellationToken cancellationToken = default)
    {
        ConversationDocument? conversationDocument;
        try
        {
            conversationDocument = await _conversationCollection.Find(
                conversationDoc => conversationDoc.GroupId == groupId, cancellationToken);
            if (conversationDocument is null)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get conversation for connection with id {id}: {ErrorMessage}", groupId, ex.Message);
            return null;
        }

        Group? group = await GetGroup(conversationDocument, cancellationToken);
        List<Message> messages = await GetMessages(conversationDocument, cancellationToken);

        return conversationDocument is not null
            ? (GroupConversation)_mapper.MapConversationFromDocument(conversationDocument, null, group, messages)
            : null;
    }

    public async Task<List<Conversation>> GetConversationsByUser(IUserAccount userAccount, CancellationToken cancellationToken = default)
    {
        IEnumerable<ConversationDocument> conversations = Enumerable.Empty<ConversationDocument>();

        try
        {
            var groupIds = (await _groupQueryRepository
                .GetGroupsByUser(userAccount, cancellationToken))
                ?.Select(g => g.GroupId)
                .ToList() ?? new List<string>();

            var connectionIds = (await _connectionQueryRepository
                .GetConnectionsForUser(userAccount, cancellationToken))
                ?.Select(c => c.ConnectionId)
                .ToList() ?? new List<string>();

            conversations = (await _conversationCollection
                .GetAll(conversationDoc => conversationDoc.ConnectionId == null
                    ? groupIds.Any(id => id == conversationDoc.GroupId)
                    : connectionIds.Any(id => id == conversationDoc.ConnectionId), cancellationToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get the conversations for user with id {UserId}: {ErrorMessage}", userAccount.Id, ex.Message);
        }

        return (await Task.WhenAll(conversations
            .Select(async conversationDoc => 
            {
                IConnection? connection = await GetConnection(conversationDoc, cancellationToken);
                Group? group = await GetGroup(conversationDoc);
                List<Message> messages = await GetMessages(conversationDoc);

                return _mapper.MapConversationFromDocument(conversationDoc, connection, group, messages);
            })))
            .ToList();
    }

    private async Task<List<Message>> GetMessages(ConversationDocument conversationDocument, CancellationToken cancellationToken = default) 
        => (await Task.WhenAll(conversationDocument.Messages
            .Select(async message => await _messageQueryRepository.GetMessage(message, cancellationToken))))
            .OfType<Message>()
            .ToList();
    
    private async Task<Group?> GetGroup(ConversationDocument conversationDocument, CancellationToken cancellationToken = default) 
        => conversationDocument.GroupId is null
            ? null
            : await _groupQueryRepository.GetGroupById(conversationDocument.GroupId, cancellationToken);

    private async Task<IConnection?> GetConnection(ConversationDocument conversationDocument, CancellationToken cancellationToken = default) => conversationDocument.ConnectionId is null
        ? null
        : await _connectionQueryRepository.GetConnection(conversationDocument.ConnectionId, cancellationToken);
}
