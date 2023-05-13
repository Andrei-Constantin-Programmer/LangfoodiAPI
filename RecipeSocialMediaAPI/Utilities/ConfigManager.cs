namespace RecipeSocialMediaAPI.Utilities
{
    public class ConfigManager : IConfigManager
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
            string? val = _configuration
                .GetSection("MongoDB")
                .GetValue<string>(keyName);

            return val == null ? "" : val.ToString();
        }
    }
}
