using CloudinaryDotNet;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Application.Services;
public class CloudinarySignatureService : ICloudinarySignatureService
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public CloudinarySignatureService(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public CloudinarySignatureDTO GenerateSignature(Cloudinary cloudinaryConnection, string? publicId = null)
    {
        Dictionary<string, object> signingParameters = new();

        if (publicId is not null)
        {
            signingParameters.Add("public_id", publicId);
        }

        long timestamp = _dateTimeProvider.Now.ToUnixTimeSeconds();
        signingParameters.Add("timestamp", timestamp);

        string signature = cloudinaryConnection.Api.SignParameters(signingParameters);

        return new(signature, timestamp);
    }
}
