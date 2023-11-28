namespace RecipeSocialMediaAPI.Application.Options;
public class CloudinaryEndpointOptions
{
    public const string CONFIGURATION_SECTION = "CloudinaryEndpoints";
    public string SingleRemoveUrl { get; set; } = string.Empty;
    public string BulkRemoveUrl { get; set; } = string.Empty;
}
