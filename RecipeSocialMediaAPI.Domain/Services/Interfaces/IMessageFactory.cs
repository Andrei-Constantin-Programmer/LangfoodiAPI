using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Services.Interfaces;
public interface IMessageFactory
{
    TextMessage CreateTextMessage(string id, IUserAccount sender, string text, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null);

    ImageMessage CreateImageMessage(string id, IUserAccount sender, IEnumerable<string> images, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null);

    public RecipeMessage CreateRecipeMessage(string id, IUserAccount sender, IEnumerable<RecipeAggregate> recipes, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null);
}