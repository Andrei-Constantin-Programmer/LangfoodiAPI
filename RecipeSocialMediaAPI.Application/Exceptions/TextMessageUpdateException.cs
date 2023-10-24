using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class TextMessageUpdateException : Exception
{
    public TextMessageUpdateException(string messageId, string reason) : base($"Cannot update text message with id {messageId}: {reason}") { }
}
