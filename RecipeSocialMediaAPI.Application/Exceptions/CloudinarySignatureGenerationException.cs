namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class CloudinarySignatureGenerationException : Exception
{
    public CloudinarySignatureGenerationException(string message) : base(message) { }
}
