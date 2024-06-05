using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ConversationNotFoundException : Exception
{
    public ConversationNotFoundException(string message) : base(message) { }

    protected ConversationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
