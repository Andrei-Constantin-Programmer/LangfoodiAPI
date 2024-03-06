using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IGroupQueryRepository
{
    Task<Group?> GetGroupById(string groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetGroupsByUser(IUserAccount userAccount, CancellationToken cancellationToken = default);
}
