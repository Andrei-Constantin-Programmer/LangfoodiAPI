using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ConversationNotFoundException : Exception
{
    public ConversationNotFoundException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected ConversationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
