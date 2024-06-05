using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class CloudinarySignatureGenerationException : Exception
{
    public CloudinarySignatureGenerationException(string message) : base(message) { }

    protected CloudinarySignatureGenerationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
