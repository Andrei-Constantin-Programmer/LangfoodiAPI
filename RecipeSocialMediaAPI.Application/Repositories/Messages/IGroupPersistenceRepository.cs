using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IGroupPersistenceRepository
{
    Task<Group> CreateGroup(string groupName, string groupDescription, List<IUserAccount> users, CancellationToken cancellationToken = default);
    Task<bool> UpdateGroup(Group group, CancellationToken cancellationToken = default);
    bool DeleteGroup(Group group);
    bool DeleteGroup(string groupId);
}
