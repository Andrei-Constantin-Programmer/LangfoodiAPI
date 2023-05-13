namespace RecipeSocialMediaAPI.Utilities
{
    public interface IConfigManager
    {
        string GetMongoSetting(string keyName);
    }
}
