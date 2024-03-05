using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeConversationRepository : IConversationQueryRepository, IConversationPersistenceRepository
{
    private readonly List<Conversation> _collection;

    public FakeConversationRepository()
    {
        _collection = new();
    }

    public ConnectionConversation? GetConversationByConnection(string connectionId) => _collection
        .FirstOrDefault(conversation => conversation is ConnectionConversation connConvo 
                                     && connConvo.Connection.ConnectionId == connectionId) as ConnectionConversation;

    public GroupConversation? GetConversationByGroup(string groupId) => _collection
        .FirstOrDefault(conversation => conversation is GroupConversation groupConvo
                                     && groupConvo.Group.GroupId == groupId) as GroupConversation;

    public Conversation? GetConversationById(string id) => _collection
        .FirstOrDefault(conversation => conversation.ConversationId == id);

    public Task<List<Conversation>> GetConversationsByUser(IUserAccount userAccount, CancellationToken cancellationToken = default) => Task.FromResult(_collection
        .Where(conversation => (conversation is ConnectionConversation connConvo 
                                && (connConvo.Connection.Account1.Id == userAccount.Id || connConvo.Connection.Account2.Id == userAccount.Id))
                            || (conversation is GroupConversation groupConvo
                                && groupConvo.Group.Users.Any(user => user.Id == userAccount.Id)))
        .ToList());

    public Conversation CreateConnectionConversation(IConnection connection)
    {
        var id = NextId();
        ConnectionConversation conversation = new(connection, id);
        _collection.Add(conversation);

        return conversation;
    }

    public Conversation CreateGroupConversation(Group group)
    {
        var id = NextId();
        GroupConversation conversation = new(group, id);
        _collection.Add(conversation);

        return conversation;
    }

    public bool UpdateConversation(Conversation conversation, IConnection? connection = null, Group? group = null)
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

        return true;
    }

    private string NextId()
    {
        return _collection.Count.ToString();
    }
}
