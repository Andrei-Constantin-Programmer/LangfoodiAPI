using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

public interface IConnection
{
    IUserAccount Account1 { get; }
    IUserAccount Account2 { get; }
    ConnectionStatus Status { get; set; }
}
