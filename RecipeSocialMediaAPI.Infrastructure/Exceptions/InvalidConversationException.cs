namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class InvalidConversationException : Exception
{
    public InvalidConversationException(string message) : base(message) { }
}
