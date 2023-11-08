using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

public class Connection : IConnection
{
    public IUserAccount Account1 { get; }
    public IUserAccount Account2 { get; }

    public ConnectionStatus Status { get; set; }

    public Connection(IUserAccount account1, IUserAccount account2, ConnectionStatus status)
    {
        if (account1.Id == account2.Id)
        {
            throw new ArgumentException($"Cannot create connection between accounts with the same Id {account1.Id}.");
        }

        Account1 = account1;
        Account2 = account2;

        Status = status;
    }
}
