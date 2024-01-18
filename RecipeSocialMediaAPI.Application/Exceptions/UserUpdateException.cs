namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UserUpdateException : Exception
{
    public UserUpdateException(string message) : base(message) { }
}
