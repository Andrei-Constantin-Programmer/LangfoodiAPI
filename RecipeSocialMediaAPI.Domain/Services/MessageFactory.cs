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

    public TextMessage CreateTextMessage(string id, User sender, string text, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null)
    {
        return new TextMessage(_dateTimeProvider, id, sender, text, sentDate, updatedDate, repliedToMessage);
    }
}
