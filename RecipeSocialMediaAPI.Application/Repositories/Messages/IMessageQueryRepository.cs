using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IMessageQueryRepository
{
    Message? GetMessage(string id);
    Task<IEnumerable<Message>> GetMessagesWithRecipe(string recipeId, CancellationToken cancellationToken = default);
}