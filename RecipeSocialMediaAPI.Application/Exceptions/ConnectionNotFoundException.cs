namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ConnectionNotFoundException : Exception
{
    public ConnectionNotFoundException(string message) : base(message) { }
}
