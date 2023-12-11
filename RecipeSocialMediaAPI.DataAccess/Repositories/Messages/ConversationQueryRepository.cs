namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConversationQueryRepository : IMessageQueryRepository
{

    public ConversationQueryRepository()
    {

    }

    public Conversation? GetConversationById(string id)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public List<Conversation> GetConversationsByUser(IUserAccount userAccount)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }
}
