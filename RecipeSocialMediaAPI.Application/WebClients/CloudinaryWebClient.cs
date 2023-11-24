using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using RecipeSocialMediaAPI.Application.Options;

namespace RecipeSocialMediaAPI.Application.WebClients;
public class CloudinaryWebClient
{
 
    public CloudinaryWebClient(IOptions<CloudinaryEndpointOptions> cloudinaryOptions)
    {

    }

    public bool RemoveHostedImage(CloudinarySignatureDTO signature, string publicId)
    {
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(
            new HttpMethod("POST"),
            $"https://api.cloudinary.com/v1_1/{_cloudinaryConfig.CloudName}/image/destroy" +
            $"?public_id={publicId}&api_key={apiKey}&signature={signature.Signature}&timestamp={signature.TimeStamp}"
        );

        var response = httpClient.Send(request);
        return response.IsSuccessStatusCode;
    }

    public bool BulkRemoveHostedImages(List<string> publicIds)
    {
        // TODO: change to use service/smth instead

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
}
