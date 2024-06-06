using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class MessageRemovalException : Exception
{
    public MessageRemovalException(string messageId) : base($"Could not remove message with id {messageId}") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected MessageRemovalException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
