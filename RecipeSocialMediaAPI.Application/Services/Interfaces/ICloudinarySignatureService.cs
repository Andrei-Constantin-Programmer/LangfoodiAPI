using CloudinaryDotNet;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;

namespace RecipeSocialMediaAPI.Application.Services.Interfaces;
public interface ICloudinarySignatureService
{
    public CloudinarySignatureDTO? GenerateSignature(Cloudinary cloudinaryConnection, string? publicId = null);
}
