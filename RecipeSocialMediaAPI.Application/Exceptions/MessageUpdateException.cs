using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class MessageUpdateException : Exception
{
    public MessageUpdateException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected MessageUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
