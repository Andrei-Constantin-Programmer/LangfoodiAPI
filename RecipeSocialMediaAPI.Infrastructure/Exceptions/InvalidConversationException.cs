using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class InvalidConversationException : Exception
{
    public InvalidConversationException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected InvalidConversationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
