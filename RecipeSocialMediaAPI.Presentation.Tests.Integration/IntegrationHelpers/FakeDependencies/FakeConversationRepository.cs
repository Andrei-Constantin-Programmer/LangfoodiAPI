using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Presentation.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeConversationRepository : IConversationQueryRepository, IConversationPersistenceRepository
{
    private readonly List<Conversation> _collection;

    public FakeConversationRepository()
    {
        _collection = new();
    }

    public async Task<ConnectionConversation?> GetConversationByConnectionAsync(string connectionId, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection
            .FirstOrDefault(conversation => conversation is ConnectionConversation connConvo 
                                         && connConvo.Connection.ConnectionId == connectionId) as ConnectionConversation);

    public async Task<GroupConversation?> GetConversationByGroupAsync(string groupId, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection
            .FirstOrDefault(conversation => conversation is GroupConversation groupConvo
                                     && groupConvo.Group.GroupId == groupId) as GroupConversation);

    public async Task<Conversation?> GetConversationByIdAsync(string id, CancellationToken cancellationToken = default) => await Task.FromResult(_collection
        .FirstOrDefault(conversation => conversation.ConversationId == id));

    public async Task<IEnumerable<Conversation>> GetConversationsByUserAsync(IUserAccount userAccount, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection
        .Where(conversation => (conversation is ConnectionConversation connConvo 
                                && (connConvo.Connection.Account1.Id == userAccount.Id || connConvo.Connection.Account2.Id == userAccount.Id))
                            || (conversation is GroupConversation groupConvo
                                && groupConvo.Group.Users.Any(user => user.Id == userAccount.Id))));

    public async Task<Conversation> CreateConnectionConversationAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var id = NextId();
        ConnectionConversation conversation = new(connection, id);
        _collection.Add(conversation);

        return await Task.FromResult(conversation);
    }

    public async Task<Conversation> CreateGroupConversationAsync(Group group, CancellationToken cancellationToken = default)
    {
        var id = NextId();
        GroupConversation conversation = new(group, id);
        _collection.Add(conversation);

        return await Task.FromResult(conversation);
    }

    public async Task<bool> UpdateConversationAsync(Conversation conversation, IConnection? connection = null, Group? group = null, CancellationToken cancellationToken = default)
    {
        Conversation? existingConversation = _collection.FirstOrDefault(convo => convo.ConversationId == conversation.ConversationId);
        if (existingConversation is null)
        {
            return false;
        }

        Conversation updatedConversation = connection is not null
            ? new ConnectionConversation(connection, conversation.ConversationId, conversation.Messages)
            : new GroupConversation(group!, conversation.ConversationId, conversation.Messages);

        _collection.Remove(existingConversation);
        _collection.Add(updatedConversation);

        return await Task.FromResult(true);
    }

    private string NextId()
    {
        return _collection.Count.ToString();
    }
}
