using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Infrastructure.Helpers;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

public class MongoCollectionFactory : IMongoCollectionFactory
{
    private readonly MongoDatabaseOptions _databaseConfiguration;

    public MongoCollectionFactory(IOptions<MongoDatabaseOptions> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }

    public IMongoCollectionWrapper<TDocument> CreateCollection<TDocument>() where TDocument : MongoDocument
    {
        return new MongoCollectionWrapper<TDocument>(_databaseConfiguration);
    }
}
