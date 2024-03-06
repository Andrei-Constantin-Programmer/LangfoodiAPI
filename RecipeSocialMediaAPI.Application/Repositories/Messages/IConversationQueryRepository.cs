using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConversationQueryRepository
{
    Task<Conversation?> GetConversationById(string id, CancellationToken cancellationToken = default);

    Task<ConnectionConversation?> GetConversationByConnection(string connectionId, CancellationToken cancellationToken = default);
    Task<GroupConversation?> GetConversationByGroup(string groupId, CancellationToken cancellationToken = default);

    Task<List<Conversation>> GetConversationsByUser(IUserAccount userAccount, CancellationToken cancellationToken = default);
}
