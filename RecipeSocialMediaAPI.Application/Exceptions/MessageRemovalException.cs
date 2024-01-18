namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class MessageRemovalException : Exception
{
    public MessageRemovalException(string messageId) : base($"Could not remove message with id {messageId}") { }
}
