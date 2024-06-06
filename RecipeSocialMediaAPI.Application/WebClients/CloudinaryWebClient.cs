using CloudinaryDotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Options;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;
using System.Net.Http.Headers;
using System.Text;

namespace RecipeSocialMediaAPI.Application.WebClients;

public class CloudinaryWebClient : ICloudinaryWebClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CloudinaryOptions _cloudinaryOptions;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<CloudinaryWebClient> _logger;

    private readonly Cloudinary _cloudinaryConnection;

    public CloudinaryWebClient(
        IHttpClientFactory clientFactory,
        IOptions<CloudinaryOptions> cloudinaryOptions,
        IDateTimeProvider dateTimeProvider,
        ILogger<CloudinaryWebClient> logger)
    {
        _httpClientFactory = clientFactory;
        _cloudinaryOptions = cloudinaryOptions.Value;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;

        _cloudinaryConnection = new(new Account(
            _cloudinaryOptions.CloudName,
            _cloudinaryOptions.ApiKey,
            _cloudinaryOptions.ApiSecret
        ));
    }

    public bool RemoveHostedImage(string publicId)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        var signature = GenerateSignature(publicId)
            ?? throw new CloudinarySignatureGenerationException($"Could not generate Cloudinary signature for image with id {publicId}");

        using var request = new HttpRequestMessage(
            new System.Net.Http.HttpMethod("POST"),
            _cloudinaryOptions.SingleRemoveUrl +
            $"?public_id={publicId}&api_key={_cloudinaryOptions.ApiKey}&signature={signature.Signature}&timestamp={signature.TimeStamp}"
        );

        var response = httpClient.Send(request);
        return response.IsSuccessStatusCode;
    }

    public bool BulkRemoveHostedImages(List<string> publicIds)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(
            new System.Net.Http.HttpMethod("DELETE"),
            _cloudinaryOptions.BulkRemoveUrl
        );

        var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_cloudinaryOptions.ApiKey}:{_cloudinaryOptions.ApiSecret}"));
        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
        request.Content = new StringContent($"public_ids[]={string.Join("&public_ids[]=", publicIds)}");
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

        var response = httpClient.Send(request);
        return response.IsSuccessStatusCode;
    }

    public CloudinarySignatureDto? GenerateSignature(string? publicId = null)
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

            string signature = _cloudinaryConnection.Api.SignParameters(signingParameters);

            return new(signature, timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was error trying to generate a cloudinary signature for id {CloudinaryId}", publicId);
            return null;
        }
    }
}
