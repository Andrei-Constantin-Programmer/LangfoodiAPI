namespace RecipeSocialMediaAPI.Core.Exceptions;

[Serializable]
public class EmailAlreadyInUseException : Exception
{
    public string Email { get; }

    public EmailAlreadyInUseException(string email)
    {
        Email = email;
    }
}
