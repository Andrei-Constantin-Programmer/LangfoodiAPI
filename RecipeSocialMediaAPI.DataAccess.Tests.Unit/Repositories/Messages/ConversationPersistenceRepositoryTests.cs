using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class ConversationPersistenceRepositoryTests
{
    private readonly ConversationPersistenceRepository _conversationPersistenceRepositorySUT;

    public ConversationPersistenceRepositoryTests()
    {
        _conversationPersistenceRepositorySUT = new ConversationPersistenceRepository();
    }
}
