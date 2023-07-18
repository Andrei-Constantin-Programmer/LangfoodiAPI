namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

[Serializable]
public class UsernameAlreadyInUseException : Exception
{
    public string Username { get; }

    public UsernameAlreadyInUseException(string username)
    {
        Username = username;
    }
}
