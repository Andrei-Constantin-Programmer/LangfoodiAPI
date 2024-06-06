using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class CorruptedMessageException : Exception
{
    public CorruptedMessageException(string? message) : base(message)
    {
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected CorruptedMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}