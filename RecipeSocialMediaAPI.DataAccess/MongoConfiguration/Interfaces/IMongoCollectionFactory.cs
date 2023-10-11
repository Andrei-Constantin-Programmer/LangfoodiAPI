using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;

public interface IMongoCollectionFactory
{
    IMongoCollectionWrapper<TDocument> CreateCollection<TDocument>() where TDocument : MongoDocument;
}