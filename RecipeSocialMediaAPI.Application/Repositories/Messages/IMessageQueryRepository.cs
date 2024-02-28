using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IMessageQueryRepository
{
    Message? GetMessage(string id);
    IEnumerable<Message> GetMessagesWithRecipe(RecipeAggregate recipe);
}