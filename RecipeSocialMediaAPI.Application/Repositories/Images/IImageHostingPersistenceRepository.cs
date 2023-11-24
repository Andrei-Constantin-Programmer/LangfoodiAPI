namespace RecipeSocialMediaAPI.Application.Repositories.Images;
public interface IImageHostingPersistenceRepository
{
    public bool RemoveHostedImage(string publicId);
    public bool BulkRemoveHostedImages(List<string> publicIds);
}
