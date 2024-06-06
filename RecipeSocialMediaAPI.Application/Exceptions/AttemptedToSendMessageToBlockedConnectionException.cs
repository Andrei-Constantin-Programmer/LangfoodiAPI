using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class AttemptedToSendMessageToBlockedConnectionException : Exception
{
    public AttemptedToSendMessageToBlockedConnectionException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected AttemptedToSendMessageToBlockedConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
