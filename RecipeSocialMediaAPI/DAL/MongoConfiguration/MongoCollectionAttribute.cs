﻿namespace RecipeSocialMediaAPI.DAL.MongoConfiguration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MongoCollectionAttribute : Attribute
    {
        public string CollectionName { get; }

        public MongoCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
