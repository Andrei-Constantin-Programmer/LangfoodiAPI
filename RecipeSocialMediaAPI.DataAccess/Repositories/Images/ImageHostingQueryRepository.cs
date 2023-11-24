using CloudinaryDotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.ImageHosting;
public class ImageHostingQueryRepository : IImageHostingQueryRepository
{
    private readonly ILogger _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly Cloudinary _connection;
    private readonly CloudinaryApiOptions _cloudinaryConfig;

    public ImageHostingQueryRepository(ILogger<ImageHostingQueryRepository> logger, IDateTimeProvider dateTimeProvider, IOptions<CloudinaryApiOptions> cloudinaryOptions)
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

    public CloudinarySignatureDTO? GenerateSignature(string? publicId = null)
    {
        try
        {
            Dictionary<string, object> signingParameters = new();

            if (publicId is not null)
            {
                signingParameters.Add("public_id", publicId);
            }

            long timestamp = _dateTimeProvider.Now.ToUnixTimeSeconds();
            signingParameters.Add("timestamp", timestamp);

            string signature = _connection.Api.SignParameters(signingParameters);
            return new() { Signature = signature, TimeStamp = timestamp };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was error trying to generate a cloudinary signature");
            return null;
        }
    }
}
