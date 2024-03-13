using RecipeSocialMediaAPI.Application.DTO.ImageHosting;

namespace RecipeSocialMediaAPI.Application.WebClients.Interfaces;

public interface ICloudinaryWebClient
{
    bool RemoveHostedImage(string publicId);
    bool BulkRemoveHostedImages(List<string> publicIds);
    CloudinarySignatureDTO? GenerateSignature(string? publicId = null);
}
