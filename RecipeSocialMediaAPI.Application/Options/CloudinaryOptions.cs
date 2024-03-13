namespace RecipeSocialMediaAPI.Application.Options;

public class CloudinaryOptions
{
    public const string CONFIGURATION_SECTION = "Cloudinary";

    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string SingleRemoveUrl { get; set; } = string.Empty;
    public string BulkRemoveUrl { get; set; } = string.Empty;
}
