using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

public abstract class Message
{
    public string Id { get; }
    public IUserAccount Sender { get; }
    public DateTimeOffset SentDate { get; }
    public DateTimeOffset? UpdatedDate { get; protected set; }

    public Message? RepliedToMessage { get; }

    internal Message(string id, IUserAccount sender, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null)
    {
        Id = id;
        Sender = sender;
        SentDate = sentDate;
        UpdatedDate = updatedDate;
        RepliedToMessage = repliedToMessage;
    }
}
