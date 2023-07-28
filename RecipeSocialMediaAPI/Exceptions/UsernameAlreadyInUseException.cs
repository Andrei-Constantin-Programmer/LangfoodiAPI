namespace RecipeSocialMediaAPI.Core.Exceptions;

[Serializable]
public class UsernameAlreadyInUseException : Exception
{
    public string Username { get; }

    public UsernameAlreadyInUseException(string username)
    {
        Username = username;
    }
}
