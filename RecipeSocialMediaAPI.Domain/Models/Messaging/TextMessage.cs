using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging;

public record TextMessage : Message
{
    private string _textContent;
    public string Text 
    { 
        get => _textContent; 
        set
        {
            ValidateTextContentAndThrow(value);

            _textContent = value;
            UpdatedDate = _dateTimeProvider.Now;
        }
    }

    internal TextMessage(IDateTimeProvider dateTimeProvider, string id, User sender, string text, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null, Message? repliedToMessage = null)
        : base(dateTimeProvider, id, sender, sentDate, updatedDate, repliedToMessage)
    {
        ValidateTextContentAndThrow(text);

        _textContent = text;
    }

    private static void ValidateTextContentAndThrow(string textContent)
    {
        if (string.IsNullOrWhiteSpace(textContent))
        {
            throw new ArgumentException("Message content (text) cannot be empty or whitespace.");
        }
    }
}
