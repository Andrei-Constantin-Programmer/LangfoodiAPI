using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
public class RemovedRecipeMessage : Message
{
    private readonly IDateTimeProvider _dateTimeProvider;

    private string? _textContent;

    public string? TextContent
    {
        get => _textContent;
        set
        {
            _textContent = value;
            UpdatedDate = _dateTimeProvider.Now;
        }
    }
    public RemovedRecipeMessage(IDateTimeProvider dateTimeProvider,
        string id, IUserAccount sender, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null, List<IUserAccount>? seenBy = null) 
        : base(id, sender, sentDate, updatedDate, repliedToMessage, seenBy)
    {
        _dateTimeProvider = dateTimeProvider;
        _textContent = textContent;
    }
}
