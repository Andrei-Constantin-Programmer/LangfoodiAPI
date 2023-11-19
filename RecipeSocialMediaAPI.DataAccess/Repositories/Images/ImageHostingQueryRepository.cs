using CloudinaryDotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.Domain.Utilities;
using System.Net.Http.Headers;
using System.Text;

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

    public bool RemoveHostedImage(string publicId)
    {
        // TODO: change to use service instead

        var signature = GenerateSignature(publicId);
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(
            new System.Net.Http.HttpMethod("POST"),
            $"https://api.cloudinary.com/v1_1/{_cloudinaryConfig.CloudName}/image/destroy" +
            $"?public_id={publicId}&api_key={_cloudinaryConfig.ApiKey}&signature={signature.Signature}&timestamp={signature.TimeStamp}"
        );

        var response = httpClient.Send(request);
        return response.IsSuccessStatusCode;
    }

    public bool BulkRemoveHostedImages(List<string> publicIds)
    {
        // TODO: change to use service instead
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(
            new System.Net.Http.HttpMethod("DELETE"), 
            $"https://api.cloudinary.com/v1_1/{_cloudinaryConfig.CloudName}/resources/image/upload"
        );

        var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_cloudinaryConfig.ApiKey}:{_cloudinaryConfig.ApiSecret}"));
        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
        request.Content = new StringContent($"public_ids[]={string.Join("&public_ids[]=", publicIds)}");
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

        var response = httpClient.Send(request);
        return response.IsSuccessStatusCode;
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
