namespace RecipeSocialMediaAPI.Infrastructure.Helpers;

public class CloudinaryApiOptions
{
    public const string CONFIGURATION_SECTION = "Cloudinary";

    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}
