using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Exceptions;

[Serializable]
public class ConnectionDocumentNotFoundException : Exception
{
    public ConnectionDocumentNotFoundException(IUserAccount user1, IUserAccount user2) : base($"Connection document between users with ids {user1.Id} and {user2.Id} not found") { }
}