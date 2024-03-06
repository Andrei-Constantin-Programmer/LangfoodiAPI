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

    public async Task<Group?> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.FirstOrDefault(group => group.GroupId == groupId));

    public Task<IEnumerable<Group>> GetGroupsByUserAsync(IUserAccount userAccount, CancellationToken cancellationToken = default) => Task.FromResult(_collection
        .Where(group => group.Users.Any(user => user.Id == userAccount.Id)));

    public async Task<Group> CreateGroupAsync(string groupName, string groupDescription, List<IUserAccount> users, CancellationToken cancellationToken = default)
    {
        var id = _collection.Count.ToString();
        Group group = new(id, groupName, groupDescription, users);
        _collection.Add(group);

        return await Task.FromResult(group);
    }

    public async Task<bool> UpdateGroupAsync(Group group, CancellationToken cancellationToken = default)
    {
        Group? existingGroup = _collection.FirstOrDefault(g => g.GroupId == group.GroupId);
        if (existingGroup is null)
        {
            return false;
        }

        Group updatedGroup = new(group.GroupId, group.GroupName, group.GroupDescription, group.Users);
        _collection.Remove(existingGroup);
        _collection.Add(updatedGroup);

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteGroupAsync(Group group, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.Remove(group));

    public async Task<bool> DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        var group = await GetGroupByIdAsync(groupId, cancellationToken);

        return group is not null && _collection.Remove(group);
    }
}
