using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class InvalidConversationException : Exception
{
    public InvalidConversationException(string message) : base(message) { }

    protected InvalidConversationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
