using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Domain.Services;

public class MessageFactory : IMessageFactory
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public MessageFactory(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public TextMessage CreateTextMessage(string id, IUserAccount sender, string textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null)
    {
        return new TextMessage(_dateTimeProvider, id, sender, textContent, sentDate, updatedDate, repliedToMessage);
    }

    public ImageMessage CreateImageMessage(string id, IUserAccount sender, IEnumerable<string> images, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null)
    {
        return new ImageMessage(_dateTimeProvider, id, sender, images, textContent, sentDate, updatedDate, repliedToMessage);
    }

    public RecipeMessage CreateRecipeMessage(string id, IUserAccount sender, IEnumerable<RecipeAggregate> recipes, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null)
    {
        return new RecipeMessage(_dateTimeProvider, id, sender, recipes, textContent, sentDate, updatedDate, repliedToMessage);
    }
}
