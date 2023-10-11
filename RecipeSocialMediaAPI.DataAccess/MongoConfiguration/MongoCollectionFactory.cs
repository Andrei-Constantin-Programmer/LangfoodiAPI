using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

public class MongoCollectionFactory : IMongoCollectionFactory
{
    private readonly DatabaseConfiguration _databaseConfiguration;

    public MongoCollectionFactory(DatabaseConfiguration databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration;
    }

    public IMongoCollectionWrapper<TDocument> CreateCollection<TDocument>() where TDocument : MongoDocument
    {
        return new MongoCollectionWrapper<TDocument>(_databaseConfiguration);
    }
}
