using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IGroupQueryRepository
{
    Group? GetGroupById(string groupId);
    IEnumerable<Group> GetGroupsByUser(IUserAccount userAccount);
}
