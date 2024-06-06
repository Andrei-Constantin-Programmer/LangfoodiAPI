using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class CloudinarySignatureGenerationException : Exception
{
    public CloudinarySignatureGenerationException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected CloudinarySignatureGenerationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
