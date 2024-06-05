using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class CorruptedMessageException : Exception
{
    public CorruptedMessageException(string? message) : base(message)
    {
    }

    protected CorruptedMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}