using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class TextMessageUpdateException : MessageUpdateException
{
    public TextMessageUpdateException(string messageId, string reason) : base($"Cannot update text message with id {messageId}: {reason}") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected TextMessageUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
