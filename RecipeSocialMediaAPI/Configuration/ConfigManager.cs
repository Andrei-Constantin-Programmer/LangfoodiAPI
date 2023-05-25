namespace RecipeSocialMediaAPI.Utilities
{
    internal class ConfigManager : IConfigManager
    {
        private readonly IConfiguration _configuration;

        public ConfigManager()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()                
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public string GetMongoSetting(string keyName)
        {
            return _configuration
                .GetSection("MongoDB")
                .GetValue<string>(keyName) ?? string.Empty;
        }
    }
}
