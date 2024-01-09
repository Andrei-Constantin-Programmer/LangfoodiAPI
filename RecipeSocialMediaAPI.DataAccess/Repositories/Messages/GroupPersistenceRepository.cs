using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class GroupPersistenceRepository : IGroupPersistenceRepository
{
    private readonly IMongoCollectionWrapper<GroupDocument> _groupCollection;
    private readonly IGroupDocumentToModelMapper _mapper;

    public GroupPersistenceRepository(IMongoCollectionFactory mongoCollectionFactory, IGroupDocumentToModelMapper mapper)
    {
        _groupCollection = mongoCollectionFactory.CreateCollection<GroupDocument>();
        _mapper = mapper;
    }

    public Group CreateGroup(string groupName, string groupDescription, List<IUserAccount> users)
    {
        GroupDocument groupDocument = _groupCollection.Insert(new()
        {
            GroupName = groupName,
            GroupDescription = groupDescription,
            UserIds = users.Select(user => user.Id).ToList()
        });

        return _mapper.MapGroupFromDocument(groupDocument);
    }

    public bool UpdateGroup(Group group) => throw new NotImplementedException();
    public bool DeleteGroup(Group group) => throw new NotImplementedException();
    public bool DeleteGroup(string groupId) => throw new NotImplementedException();
}
