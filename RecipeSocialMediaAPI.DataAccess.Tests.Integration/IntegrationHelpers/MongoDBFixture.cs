using Mongo2Go;
using MongoDB.Driver;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.Tests.Shared.TestHelpers;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Integration.IntegrationHelpers;

public class MongoDBFixture : IDisposable
{
    public MongoClient Client { get; }
    public IMongoDatabase Database { get; }
    public string ConnectionString { get; }
    public string DatabaseName { get; } = "testdb";
    
    private readonly MongoDbRunner _mongoRunner;

    public IMongoCollection<TestDocument> TestCollection { get; }
    
    public MongoDBFixture()
    {
        _mongoRunner = MongoDbRunner.Start();
        ConnectionString = _mongoRunner.ConnectionString;
        Client = new MongoClient(ConnectionString);
        Database = Client.GetDatabase(DatabaseName);

        TestCollection = GetTestCollection(Database);
    }

    public void CleanupCollection()
    {
        TestCollection.DeleteMany(_ => true);
    }

    public void Dispose()
    {
        _mongoRunner.Dispose();
        GC.SuppressFinalize(this);
    }

    private static IMongoCollection<TestDocument> GetTestCollection(IMongoDatabase database) => database
        .GetCollection<TestDocument>(((MongoCollectionAttribute)typeof(TestDocument).GetCustomAttributes(typeof(MongoCollectionAttribute), true)[0]).CollectionName);
}
