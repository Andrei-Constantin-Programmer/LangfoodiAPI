using CloudinaryDotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.Helpers;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.ImageHosting;
public class ImageHostingQueryRepository : IImageHostingQueryRepository
{
    private readonly ILogger _logger;
    private readonly ICloudinarySignatureService _cloudinarySignatureService;
    private readonly Cloudinary _connection;
    private readonly CloudinaryApiOptions _cloudinaryConfig;

    public ImageHostingQueryRepository(ICloudinarySignatureService signatureService, ILogger<ImageHostingQueryRepository> logger, IOptions<CloudinaryApiOptions> cloudinaryOptions)
    {
        _cloudinarySignatureService = signatureService;
        _logger = logger;

        _cloudinaryConfig = cloudinaryOptions.Value;
        _connection = new Cloudinary(new Account(
            _cloudinaryConfig.CloudName,
            _cloudinaryConfig.ApiKey,
            _cloudinaryConfig.ApiSecret
        ));
    }

    public CloudinarySignatureDTO? GenerateSignature(string? publicId = null)
    {
        try
        {
            return _cloudinarySignatureService.GenerateSignature(_connection, publicId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was error trying to generate a cloudinary signature");
            return null;
        }
    }
}
