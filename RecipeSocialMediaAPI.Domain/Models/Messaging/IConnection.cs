using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging;

public interface IConnection
{
    IUserAccount Account1 { get; }
    IUserAccount Account2 { get; }
    ConnectionStatus Status { get; set; }
    bool BindConversation(ConnectionConversation conversation);
}
