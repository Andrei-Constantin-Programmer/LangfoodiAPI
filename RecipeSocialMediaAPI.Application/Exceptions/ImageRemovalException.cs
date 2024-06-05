using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ImageRemovalException : Exception
{
    public ImageRemovalException(string publicId) : base($"Could not remove image with publicId: {publicId}") { }

    protected ImageRemovalException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
