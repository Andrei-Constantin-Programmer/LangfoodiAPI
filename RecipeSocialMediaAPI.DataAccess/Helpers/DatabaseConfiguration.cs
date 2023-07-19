namespace RecipeSocialMediaAPI.DataAccess.Helpers;

public record DatabaseConfiguration
(
    string MongoConnectionString,
    string MongoClusterName
);
