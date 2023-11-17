using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IGroupPersistenceRepository
{
    public Group CreateGroup(string groupName, string groupDescription, List<IUserAccount> users);
    public bool UpdateGroup(Group group);
    public bool DeleteGroup(Group group);
    public bool DeleteGroup(string groupId);
}
