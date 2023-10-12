using CloudinaryDotNet;
using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.ImageHosting;
public class ImageHostingQueryRepository : IImageHostingQueryRepository
{
    private readonly ILogger _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CloudinaryApiConfiguration _cloudinaryConfiguration;
    private readonly Cloudinary _connection;    

    public ImageHostingQueryRepository(ILogger<ImageHostingQueryRepository> logger, IDateTimeProvider dateTimeProvider, CloudinaryApiConfiguration cloudinaryConfiguration)
    {
        _dateTimeProvider = dateTimeProvider;
        _cloudinaryConfiguration = cloudinaryConfiguration;
        _logger = logger;

        _connection = new Cloudinary(new Account(
            _cloudinaryConfiguration.CloudName,
            _cloudinaryConfiguration.ApiKey,
            _cloudinaryConfiguration.ApiSecret
        ));
    }

    public CloudinarySignatureDTO? GenerateClientSignature()
    {
        try
        {
            long timestamp = _dateTimeProvider.Now.ToUnixTimeSeconds();
            Dictionary<string, object> sigParams = new Dictionary<string, object>() {
                { "timestamp", timestamp }
            };

            string signature = _connection.Api.SignParameters(sigParams);           
            return new() { Signature = signature, TimeStamp = timestamp };
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "There was error trying to generate a cloudinary client signature");
            return null;
        }
    }
}
