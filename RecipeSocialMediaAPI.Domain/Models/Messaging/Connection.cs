using RecipeSocialMediaAPI.Domain.Models.Messaging.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging;

public class Connection : IConnection
{
    public IUserAccount Account1 { get; }
    public IUserAccount Account2 { get; }

    public ConnectionStatus Status { get; set; }

    public Connection(IUserAccount account1, IUserAccount account2, ConnectionStatus status)
    {
        Account1 = account1;
        Account2 = account2;

        Status = status;
    }
}
