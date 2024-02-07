using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

public abstract class Message
{
    public string Id { get; }
    public IUserAccount Sender { get; }
    public DateTimeOffset SentDate { get; }
    public DateTimeOffset? UpdatedDate { get; protected set; }

    public Message? RepliedToMessage { get; }

    private readonly HashSet<IUserAccount> _seenBy;
    public List<IUserAccount> SeenBy => _seenBy.ToList();

    internal Message(string id, IUserAccount sender, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null, List<IUserAccount>? seenBy = null)
    {
        Id = id;
        Sender = sender;
        SentDate = sentDate;
        UpdatedDate = updatedDate;
        RepliedToMessage = repliedToMessage;
        _seenBy = seenBy?.ToHashSet() ?? new();
    }

    public bool MarkAsSeenBy(IUserAccount user) => _seenBy.Add(user);
}
