using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Services.Interfaces;
public interface IMessageFactory
{
    Message CreateTextMessage(string id, IUserAccount sender, string text, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null);

    Message CreateImageMessage(string id, IUserAccount sender, IEnumerable<string> images, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null);

    public Message CreateRecipeMessage(string id, IUserAccount sender, IEnumerable<RecipeAggregate> recipes, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null);
}