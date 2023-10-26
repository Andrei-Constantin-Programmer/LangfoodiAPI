namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class TextMessageUpdateException : MessageUpdateException
{
    public TextMessageUpdateException(string messageId, string reason) : base($"Cannot update text message with id {messageId}: {reason}") { }
}
