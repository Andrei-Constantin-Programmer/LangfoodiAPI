using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConversationQueryRepository
{
    Conversation? GetConversationById(string id);

    ConnectionConversation? GetConversationByConnection(string connectionId);
    GroupConversation? GetConversationByGroup(string groupId);

    List<Conversation> GetConversationsByUser(IUserAccount userAccount);
}
