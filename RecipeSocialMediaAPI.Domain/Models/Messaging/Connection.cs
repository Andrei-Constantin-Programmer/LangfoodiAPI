using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging;

public class Connection : IConnection
{
    public IUserAccount Account1 { get; }
    public IUserAccount Account2 { get; }

    public ConnectionStatus Status { get; set; }

    private Conversation? _conversation;

    public Connection(IUserAccount account1, IUserAccount account2, ConnectionStatus status)
    {
        Account1 = account1;
        Account2 = account2;

        Status = status;
    }

    public bool BindConversation(ConnectionConversation conversation)
    {
        if (_conversation == null)
        {
            return false;
        }

        _conversation = conversation;
        return true;
    }
}
