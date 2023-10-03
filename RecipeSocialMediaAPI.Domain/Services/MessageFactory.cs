using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.Domain.Services;

public class MessageFactory : IMessageFactory
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public MessageFactory(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public TextMessage CreateTextMessage(string id, User sender, string textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null)
    {
        return new TextMessage(_dateTimeProvider, id, sender, textContent, sentDate, updatedDate, repliedToMessage);
    }

    public ImageMessage CreateImageMessage(string id, User sender, IEnumerable<string> images, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null)
    {
        return new ImageMessage(_dateTimeProvider, id, sender, images, textContent, sentDate, updatedDate, repliedToMessage);
    }
}
