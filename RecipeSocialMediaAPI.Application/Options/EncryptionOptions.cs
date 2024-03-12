namespace RecipeSocialMediaAPI.Application.Options;

public class EncryptionOptions
{
    public const string CONFIGURATION_SECTION = "Encryption";

    public string EncryptionKey { get; set; } = string.Empty;
}
