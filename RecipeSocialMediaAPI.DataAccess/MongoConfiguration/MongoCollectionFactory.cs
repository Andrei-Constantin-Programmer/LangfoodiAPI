using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

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
