using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class GroupPersistenceRepository : IGroupPersistenceRepository
{
    private readonly IMongoCollectionWrapper<GroupDocument> _groupCollection;

    public GroupPersistenceRepository(IMongoCollectionFactory mongoCollectionFactory)
    {
        _groupCollection = mongoCollectionFactory.CreateCollection<GroupDocument>();
    }

    public Group CreateGroup(string groupName, string groupDescription, List<IUserAccount> users) => throw new NotImplementedException();
    public bool UpdateGroup(Group group) => throw new NotImplementedException();
    public bool DeleteGroup(Group group) => throw new NotImplementedException();
    public bool DeleteGroup(string groupId) => throw new NotImplementedException();
}
