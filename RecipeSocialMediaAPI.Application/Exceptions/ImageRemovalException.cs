namespace RecipeSocialMediaAPI.Application.Exceptions;
public class ImageRemovalException : Exception
{
    public ImageRemovalException(string publicId) : base($"Could not remove image with publicId: {publicId}") { }
}
