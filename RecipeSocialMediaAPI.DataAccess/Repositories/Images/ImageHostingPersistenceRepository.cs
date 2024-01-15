using CloudinaryDotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Images;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Helpers;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Images;
public class ImageHostingPersistenceRepository : IImageHostingPersistenceRepository
{
    private readonly ILogger _logger;
    private readonly ICloudinarySignatureService _cloudinarySignatureService;
    private readonly ICloudinaryWebClient _cloudinaryWebClient;
    private readonly Cloudinary _connection;
    private readonly CloudinaryApiOptions _cloudinaryConfig;

    public ImageHostingPersistenceRepository(ICloudinaryWebClient cloudinaryWebClient,  ICloudinarySignatureService signatureService, ILogger<ImageHostingPersistenceRepository> logger, IOptions<CloudinaryApiOptions> cloudinaryOptions)
    {
        _cloudinaryWebClient = cloudinaryWebClient;
        _cloudinarySignatureService = signatureService;
        _logger = logger;

        _cloudinaryConfig = cloudinaryOptions.Value;
        _connection = new Cloudinary(new Account(
            _cloudinaryConfig.CloudName,
            _cloudinaryConfig.ApiKey,
            _cloudinaryConfig.ApiSecret
        ));
    }

    public bool RemoveHostedImage(string publicId)
    {
        try
        {
            var signature = _cloudinarySignatureService.GenerateSignature(_connection, publicId);

            return _cloudinaryWebClient.RemoveHostedImage(
                signature,
                _cloudinaryConfig.ApiKey,
                publicId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was error trying to remove the image: {ImagePublicId}", publicId);
            return false;
        }
    }

    public bool BulkRemoveHostedImages(List<string> publicIds)
    {
        try
        {
            return _cloudinaryWebClient.BulkRemoveHostedImages(
                publicIds, 
                _cloudinaryConfig.ApiKey,
                _cloudinaryConfig.ApiSecret
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"There was error trying to bulk remove given images");
            return false;
        }
    }
}
