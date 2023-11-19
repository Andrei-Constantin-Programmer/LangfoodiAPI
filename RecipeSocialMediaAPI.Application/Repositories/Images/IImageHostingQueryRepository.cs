using RecipeSocialMediaAPI.Application.DTO.ImageHosting;

namespace RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
public interface IImageHostingQueryRepository
{
    public CloudinarySignatureDTO? GenerateSignature(string? publicId = null);
    public bool BulkRemoveHostedImages(List<string> publicIds);
    public bool RemoveHostedImage(string publicId);
}
