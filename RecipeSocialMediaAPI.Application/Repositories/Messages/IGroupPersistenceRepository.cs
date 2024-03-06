using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IGroupPersistenceRepository
{
    Task<Group> CreateGroupAsync(string groupName, string groupDescription, List<IUserAccount> users, CancellationToken cancellationToken = default);
    Task<bool> UpdateGroupAsync(Group group, CancellationToken cancellationToken = default);
    Task<bool> DeleteGroupAsync(Group group, CancellationToken cancellationToken = default);
    Task<bool> DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default);
}
