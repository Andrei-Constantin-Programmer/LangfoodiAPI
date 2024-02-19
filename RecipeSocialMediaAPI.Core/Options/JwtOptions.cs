namespace RecipeSocialMediaAPI.Core.Options;

public class JwtOptions
{
    public const string CONFIGURATION_SECTION = "JwtSettings";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
