using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

public class TextMessage : Message
{
    private readonly IDateTimeProvider _dateTimeProvider;

    private string _textContent;
    public string TextContent 
    { 
        get => _textContent; 
        set
        {
            ValidateTextContentAndThrow(value);

            _textContent = value;
            UpdatedDate = _dateTimeProvider.Now;
        }
    }

    internal TextMessage(IDateTimeProvider dateTimeProvider, 
        string id, IUserAccount sender, string textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null)
        : base(id, sender, sentDate, updatedDate, repliedToMessage)
    {
        _dateTimeProvider = dateTimeProvider;

        ValidateTextContentAndThrow(textContent);

        _textContent = textContent;
    }

    private static void ValidateTextContentAndThrow(string textContent)
    {
        if (string.IsNullOrWhiteSpace(textContent))
        {
            throw new ArgumentException("Message content (text) cannot be empty or whitespace.");
        }
    }
}
