using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IGroupQueryRepository
{
    Task<Group?> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetGroupsByUserAsync(IUserAccount userAccount, CancellationToken cancellationToken = default);
}
