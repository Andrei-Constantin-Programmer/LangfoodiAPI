using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IMessageQueryRepository
{
    Task<Message?> GetMessageAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetMessagesWithRecipeAsync(string recipeId, CancellationToken cancellationToken = default);
}