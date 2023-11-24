using CloudinaryDotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Application.Repositories.Images;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Repositories.ImageHosting;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Images;
public class ImageHostingPersistenceRepository : IImageHostingPersistenceRepository
{
    private readonly ILogger _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly Cloudinary _connection;
    private readonly CloudinaryApiOptions _cloudinaryConfig;

    public ImageHostingPersistenceRepository(ILogger<ImageHostingQueryRepository> logger, IDateTimeProvider dateTimeProvider, IOptions<CloudinaryApiOptions> cloudinaryOptions)
    {
        _dateTimeProvider = dateTimeProvider;
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
    }

    public bool BulkRemoveHostedImages(List<string> publicIds)
    {
    }
}
