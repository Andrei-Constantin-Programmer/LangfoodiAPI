using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeMongoCollectionFactory : IMongoCollectionFactory
{
    public IMongoRepository<TDocument> GetCollection<TDocument>() where TDocument : MongoDocument
    => new FakeRepository<TDocument>();
}

internal class FakeRepository<TDocument> : IMongoRepository<TDocument> where TDocument : MongoDocument
{
    private readonly List<TDocument> _collection;

    public FakeRepository()
    {
        _collection = new List<TDocument>();
    }

    public bool Contains(Expression<Func<TDocument, bool>> expr) 
        => _collection.Any(user => expr.Compile()(user));
    public bool Delete(Expression<Func<TDocument, bool>> expr) 
        => _collection.RemoveAll(user => expr.Compile()(user)) > 0;
    public TDocument? Find(Expression<Func<TDocument, bool>> expr) 
        => _collection.SingleOrDefault(user => expr.Compile()(user));
    public List<TDocument> GetAll(Expression<Func<TDocument, bool>> expr) 
        => _collection.Where(user => expr.Compile()(user)).ToList();
    public TDocument Insert(TDocument doc)
    {
        doc.Id = _collection.Count.ToString();

        _collection.Add(doc);

        return doc; 
    }
    public bool UpdateRecord(TDocument record, Expression<Func<TDocument, bool>> expr)
    {
        try
        {
            var userToChange = _collection.Single(user => expr.Compile()(user));
            userToChange = record;

            return true;
        } catch (Exception)
        {
            return false;
        }
    }
}
