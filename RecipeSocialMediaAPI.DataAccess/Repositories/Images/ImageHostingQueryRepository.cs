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

    public ImageHostingQueryRepository(ILogger<ImageHostingQueryRepository> logger, IDateTimeProvider dateTimeProvider, IOptions<CloudinaryApiOptions> cloudinaryOptions)
    {
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;

        var cloudinaryConfiguration = cloudinaryOptions.Value;
        _connection = new Cloudinary(new Account(
            cloudinaryConfiguration.CloudName,
            cloudinaryConfiguration.ApiKey,
            cloudinaryConfiguration.ApiSecret
        ));
    }

    public CloudinarySignatureDTO? GenerateClientSignature(string? publicId)
    {
        Dictionary<string, object> signingParameters = new();

        if (publicId is not null)
        {
            signingParameters.Add("public_id", publicId);
        }

        return GenerateClientSignature(signingParameters);
    }

    public CloudinarySignatureDTO? GenerateClientSignature(List<string> publicIds)
    {
        Dictionary<string, object> signingParameters = new();

        for (int i=0; i<publicIds.Count; i++)
        {
            signingParameters.Add($"public_ids[{i}]", publicIds[i]);
        }

        return GenerateClientSignature(signingParameters);
    }

    private CloudinarySignatureDTO? GenerateClientSignature(Dictionary<string, object> signingParameters)
    {
        try
        {
            long timestamp = _dateTimeProvider.Now.ToUnixTimeSeconds();
            signingParameters.Add("timestamp", timestamp);

            string signature = _connection.Api.SignParameters(signingParameters);
            return new() { Signature = signature, TimeStamp = timestamp };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was error trying to generate a cloudinary client signature");
            return null;
        }
    }
}
