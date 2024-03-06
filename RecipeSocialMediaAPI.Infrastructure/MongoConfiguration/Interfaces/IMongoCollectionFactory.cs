using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;

public interface IMongoCollectionFactory
{
    IMongoCollectionWrapper<TDocument> CreateCollection<TDocument>() where TDocument : MongoDocument;
}