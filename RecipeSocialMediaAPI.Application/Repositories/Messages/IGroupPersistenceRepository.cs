using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IGroupPersistenceRepository
{
    Task<Group> CreateGroup(string groupName, string groupDescription, List<IUserAccount> users, CancellationToken cancellationToken = default);
    bool UpdateGroup(Group group);
    bool DeleteGroup(Group group);
    bool DeleteGroup(string groupId);
}
