using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Options;
using System.Net.Http.Headers;
using System.Text;

namespace RecipeSocialMediaAPI.Application.WebClients;
public class CloudinaryWebClient : ICloudinaryWebClient
{
    private readonly CloudinaryEndpointOptions _cloudinaryEndpoints;
 
    public CloudinaryWebClient(IOptions<CloudinaryEndpointOptions> cloudinaryOptions)
    {
        _cloudinaryEndpoints = cloudinaryOptions.Value;
    }

    public bool RemoveHostedImage(CloudinarySignatureDTO signature, string apiKey, string publicId)
    {
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(
            new HttpMethod("POST"),
            _cloudinaryEndpoints.SingleRemoveUrl +
            $"?public_id={publicId}&api_key={apiKey}&signature={signature.Signature}&timestamp={signature.TimeStamp}"
        );

        var response = httpClient.Send(request);
        return response.IsSuccessStatusCode;
    }

    public bool BulkRemoveHostedImages(List<string> publicIds, string apiKey, string apiSecret)
    {
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(
            new HttpMethod("DELETE"),
            _cloudinaryEndpoints.BulkRemoveUrl
        );

        var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}"));
        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
        request.Content = new StringContent($"public_ids[]={string.Join("&public_ids[]=", publicIds)}");
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

        var response = httpClient.Send(request);
        return response.IsSuccessStatusCode;
    }
}
