using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Tests.Shared;
public class TestConnection : IConnection
{
    required public string ConnectionId { get; set;}
    required public IUserAccount Account1 { get; set; }
    required public IUserAccount Account2 { get; set; }
    public ConnectionStatus Status { get; set; } // TODO: Should this be required
}