using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class AttemptedToSendMessageToBlockedConnectionException : Exception
{
    public AttemptedToSendMessageToBlockedConnectionException(string message) : base(message) { }

    protected AttemptedToSendMessageToBlockedConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
