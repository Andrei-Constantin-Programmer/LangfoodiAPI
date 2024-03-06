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

    public async Task<Group> CreateGroup(string groupName, string groupDescription, List<IUserAccount> users, CancellationToken cancellationToken = default)
    {
        GroupDocument groupDocument = await _groupCollection.Insert(new(groupName, groupDescription, users.Select(user => user.Id).ToList()), cancellationToken);

        return await _mapper.MapGroupFromDocument(groupDocument, cancellationToken);
    }

    public async Task<bool> UpdateGroup(Group group, CancellationToken cancellationToken = default)
    {
        return await _groupCollection.UpdateRecord(
            new GroupDocument(group.GroupName, group.GroupDescription, group.Users.Select(user => user.Id).ToList(), group.GroupId),
            groupDoc => groupDoc.Id == group.GroupId, cancellationToken);
    }

    public async Task<bool> DeleteGroup(Group group, CancellationToken cancellationToken = default) 
        => await DeleteGroup(group.GroupId, cancellationToken);

    public async Task<bool> DeleteGroup(string groupId, CancellationToken cancellationToken = default) 
        => await _groupCollection.Delete(groupDoc => groupDoc.Id == groupId, cancellationToken);
}
