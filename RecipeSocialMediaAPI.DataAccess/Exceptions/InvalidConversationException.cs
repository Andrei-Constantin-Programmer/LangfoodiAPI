namespace RecipeSocialMediaAPI.DataAccess.Exceptions;

[Serializable]
public class InvalidConversationException : Exception
{
    public InvalidConversationException(string message) : base(message) { }
}
