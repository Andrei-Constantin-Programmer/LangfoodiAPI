using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConversationQueryRepository
{
    Task<Conversation?> GetConversationByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<ConnectionConversation?> GetConversationByConnectionAsync(string connectionId, CancellationToken cancellationToken = default);
    Task<GroupConversation?> GetConversationByGroupAsync(string groupId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Conversation>> GetConversationsByUserAsync(IUserAccount userAccount, CancellationToken cancellationToken = default);
}
