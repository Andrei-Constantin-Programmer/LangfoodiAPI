using RecipeSocialMediaAPI.Application.DTO.ImageHosting;

namespace RecipeSocialMediaAPI.Application.WebClients.Interfaces;
public interface ICloudinaryWebClient
{
    public bool RemoveHostedImage(CloudinarySignatureDTO signature, string apiKey, string publicId);
    public bool BulkRemoveHostedImages(List<string> publicIds, string apiKey, string apiSecret);
}
