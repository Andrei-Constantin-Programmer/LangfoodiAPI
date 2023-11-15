using RecipeSocialMediaAPI.Domain.Models.Messaging;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IGroupPersistenceRepository
{
    public Group CreateGroup(Group group);
    public bool UpdateGroup(Group group);
    public bool DeleteGroup(Group group);
    public bool DeleteGroup(string groupId);
}
