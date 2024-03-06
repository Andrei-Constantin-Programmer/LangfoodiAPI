using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IMessageQueryRepository
{
    Task<Message?> GetMessage(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetMessagesWithRecipe(string recipeId, CancellationToken cancellationToken = default);
}