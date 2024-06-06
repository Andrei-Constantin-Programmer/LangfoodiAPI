using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ImageMessageUpdateException : MessageUpdateException
{
    public ImageMessageUpdateException(string messageId, string reason) : base($"Cannot update image message with id {messageId}: {reason}") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected ImageMessageUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
