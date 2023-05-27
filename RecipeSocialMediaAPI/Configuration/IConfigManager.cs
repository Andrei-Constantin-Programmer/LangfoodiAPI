namespace RecipeSocialMediaAPI.Utilities
{
    internal interface IConfigManager
    {
        string GetMongoSetting(string keyName);
    }
}
