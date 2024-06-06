using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ImageRemovalException : Exception
{
    public ImageRemovalException(string publicId) : base($"Could not remove image with publicId: {publicId}") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected ImageRemovalException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
