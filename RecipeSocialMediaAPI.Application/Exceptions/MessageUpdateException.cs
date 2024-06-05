using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class MessageUpdateException : Exception
{
    public MessageUpdateException(string message) : base(message) { }

    protected MessageUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
