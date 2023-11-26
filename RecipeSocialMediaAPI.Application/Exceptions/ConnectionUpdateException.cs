namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ConnectionUpdateException : Exception
{
    public ConnectionUpdateException(string message) : base(message) { }
}
