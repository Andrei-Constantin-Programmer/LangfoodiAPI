namespace RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal class MongoCollectionAttribute : Attribute
{
    public string CollectionName { get; }

    public MongoCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}
