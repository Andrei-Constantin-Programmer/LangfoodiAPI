namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConversationQueryRepository
{
    Conversation? GetConversationById(string id);

    List<Conversation> GetConversationsByUser(IUserAccount userAccount);
}
