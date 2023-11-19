using RecipeSocialMediaAPI.Application.DTO.ImageHosting;

namespace RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
public interface IImageHostingQueryRepository
{
    public CloudinarySignatureDTO? GenerateClientSignature(string? publicId);
    public bool BulkRemoveHostedImages(List<string> publicIds);
    public bool RemoveHostedImage(string publicId);
}
