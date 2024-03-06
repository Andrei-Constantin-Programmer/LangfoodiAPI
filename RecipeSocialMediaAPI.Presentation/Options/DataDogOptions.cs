namespace RecipeSocialMediaAPI.Presentation.Options;

public class DataDogOptions
{
    public const string CONFIGURATION_SECTION = "DataDog";

    public string ApiKey { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
