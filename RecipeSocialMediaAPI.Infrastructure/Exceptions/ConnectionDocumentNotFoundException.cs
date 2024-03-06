namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class UserDocumentNotFoundException : Exception
{
    public UserDocumentNotFoundException(string userId) : base($"User document for user with the id {userId} was not found") { }
}