using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeGroupRepository : IGroupQueryRepository, IGroupPersistenceRepository
{
    private readonly List<Group> _collection;

    public FakeGroupRepository()
    {
        _collection = new();
    }

    public Group? GetGroupById(string groupId) => _collection.FirstOrDefault(group => group.GroupId == groupId);

    public IEnumerable<Group> GetGroupsByUser(IUserAccount userAccount) => _collection
        .Where(group => group.Users.Any(user => user.Id == userAccount.Id));

    public Group CreateGroup(string groupName, string groupDescription, List<IUserAccount> users)
    {
        var id = _collection.Count.ToString();
        Group group = new(id, groupName, groupDescription, users);
        _collection.Add(group);

        return group;
    }

    public bool UpdateGroup(Group group)
    {
        Group? existingGroup = _collection.FirstOrDefault(g => g.GroupId == group.GroupId);
        if (existingGroup is null)
        {
            return false;
        }

        Group updatedGroup = new(group.GroupId, group.GroupName, group.GroupDescription, group.Users);
        _collection.Remove(existingGroup);
        _collection.Add(updatedGroup);

        return true;
    }

    public bool DeleteGroup(Group group) => _collection.Remove(group);

    public bool DeleteGroup(string groupId)
    {
        var group = GetGroupById(groupId);

        return group is not null && _collection.Remove(group);
    }
}
