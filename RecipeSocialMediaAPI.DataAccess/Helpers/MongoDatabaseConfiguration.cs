namespace RecipeSocialMediaAPI.DataAccess.Helpers;

public record MongoDatabaseConfiguration
(
    string MongoConnectionString,
    string MongoDatabaseName
);
