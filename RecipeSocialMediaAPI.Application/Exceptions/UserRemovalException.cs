namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UserRemovalException : Exception
{
    public UserRemovalException(string userId) : base($"Could not remove user with id {userId}") { }
}
