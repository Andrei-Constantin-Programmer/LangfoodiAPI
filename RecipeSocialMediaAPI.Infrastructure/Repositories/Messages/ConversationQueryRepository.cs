﻿using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;

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

    public async Task<Conversation?> GetConversationByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ConversationDocument? conversationDocument;
        try
        {
            conversationDocument = await _conversationCollection.GetOneAsync(
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

        IConnection? connection = await GetConnectionAsync(conversationDocument, cancellationToken);
        Group? group = await GetGroupAsync(conversationDocument, cancellationToken);
        List<Message> messages = await GetMessagesAsync(conversationDocument, cancellationToken);

        return _mapper.MapConversationFromDocument(conversationDocument, connection, group, messages);
    }

    public async Task<ConnectionConversation?> GetConversationByConnectionAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        ConversationDocument? conversationDocument;
        try
        {
            conversationDocument = await _conversationCollection.GetOneAsync(
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

        IConnection? connection = await GetConnectionAsync(conversationDocument, cancellationToken);
        List<Message> messages = await GetMessagesAsync(conversationDocument, cancellationToken);

        return (ConnectionConversation)_mapper.MapConversationFromDocument(conversationDocument, connection, null, messages);
    }

    public async Task<GroupConversation?> GetConversationByGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        ConversationDocument? conversationDocument;
        try
        {
            conversationDocument = await _conversationCollection.GetOneAsync(
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

        Group? group = await GetGroupAsync(conversationDocument, cancellationToken);
        List<Message> messages = await GetMessagesAsync(conversationDocument, cancellationToken);

        return (GroupConversation)_mapper.MapConversationFromDocument(conversationDocument, null, group, messages);
    }

    public async Task<IEnumerable<Conversation>> GetConversationsByUserAsync(IUserAccount userAccount, CancellationToken cancellationToken = default)
    {
        IEnumerable<ConversationDocument> conversations = Enumerable.Empty<ConversationDocument>();

        try
        {
            var groupIds = (await _groupQueryRepository
                .GetGroupsByUserAsync(userAccount, cancellationToken))?
                .Select(g => g.GroupId)
                .ToList() ?? new List<string>();

            var connectionIds = (await _connectionQueryRepository
                .GetConnectionsForUserAsync(userAccount, cancellationToken))?
                .Select(c => c.ConnectionId)
                .ToList() ?? new List<string>();

            conversations = await _conversationCollection
                .GetAllAsync(conversationDoc => conversationDoc.ConnectionId == null
                    ? groupIds.Any(id => id == conversationDoc.GroupId)
                    : connectionIds.Any(id => id == conversationDoc.ConnectionId), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get the conversations for user with id {UserId}: {ErrorMessage}", userAccount.Id, ex.Message);
        }

        return (await Task.WhenAll(conversations
            .Select(async conversationDoc =>
            {
                IConnection? connection = await GetConnectionAsync(conversationDoc, cancellationToken);
                Group? group = await GetGroupAsync(conversationDoc);
                List<Message> messages = await GetMessagesAsync(conversationDoc);

                try
                {
                    return _mapper.MapConversationFromDocument(conversationDoc, connection, group, messages);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was an error mapping conversation {ConversationId}", conversationDoc.Id);
                    return null;
                }
            })))
            .OfType<Conversation>();
    }

    private async Task<List<Message>> GetMessagesAsync(ConversationDocument conversationDocument, CancellationToken cancellationToken = default)
        => (await Task.WhenAll(conversationDocument.Messages
            .Select(async message => await _messageQueryRepository.GetMessageAsync(message, cancellationToken))))
            .OfType<Message>()
            .ToList();

    private async Task<Group?> GetGroupAsync(ConversationDocument conversationDocument, CancellationToken cancellationToken = default)
        => conversationDocument.GroupId is null
            ? null
            : await _groupQueryRepository.GetGroupByIdAsync(conversationDocument.GroupId, cancellationToken);

    private async Task<IConnection?> GetConnectionAsync(ConversationDocument conversationDocument, CancellationToken cancellationToken = default) => conversationDocument.ConnectionId is null
        ? null
        : await _connectionQueryRepository.GetConnectionAsync(conversationDocument.ConnectionId, cancellationToken);
}
