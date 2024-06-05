using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Domain.Services;

public class MessageFactory : IMessageFactory
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public MessageFactory(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Message CreateTextMessage(
        string id,
        IUserAccount sender,
        string text,
        List<IUserAccount> seenBy,
        DateTimeOffset sentDate,
        DateTimeOffset? updatedDate = null,
        Message? repliedToMessage = null)
    {
        return new TextMessage(_dateTimeProvider, id, sender, text, sentDate, updatedDate, repliedToMessage, seenBy);
    }

    public Message CreateImageMessage(
        string id,
        IUserAccount sender,
        IEnumerable<string> images,
        string? text,
        List<IUserAccount> seenBy,
        DateTimeOffset sentDate,
        DateTimeOffset? updatedDate = null,
        Message? repliedToMessage = null)
    {
        return new ImageMessage(_dateTimeProvider, id, sender, images, text, sentDate, updatedDate, repliedToMessage, seenBy);
    }

    public Message CreateRecipeMessage(
        string id,
        IUserAccount sender,
        IEnumerable<Recipe> recipes,
        string? text,
        List<IUserAccount> seenBy,
        DateTimeOffset sentDate,
        DateTimeOffset? updatedDate = null,
        Message? repliedToMessage = null)
    {
        return new RecipeMessage(_dateTimeProvider, id, sender, recipes, text, sentDate, updatedDate, repliedToMessage, seenBy);
    }
}
