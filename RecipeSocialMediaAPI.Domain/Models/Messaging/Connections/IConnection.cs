using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

public interface IConnection
{
    string ConnectionId { get; }
    IUserAccount Account1 { get; }
    IUserAccount Account2 { get; }
    ConnectionStatus Status { get; set; }
}
