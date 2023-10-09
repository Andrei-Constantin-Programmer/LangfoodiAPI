using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RecipeSocialMediaAPI.Domain.Tests.Unit")]

namespace RecipeSocialMediaAPI.Domain.Models.Messaging;

public abstract record Message
{
    protected readonly IDateTimeProvider _dateTimeProvider;

    public string Id { get; }
    public IUserAccount Sender { get; }
    public DateTimeOffset SentDate { get; }
    public DateTimeOffset? UpdatedDate { get; protected set; }

    public Message? RepliedToMessage { get; }

    internal Message(IDateTimeProvider dateTimeProvider, 
        string id, IUserAccount sender, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null)
    {
        _dateTimeProvider = dateTimeProvider;
        Id = id;
        Sender = sender;
        SentDate = sentDate;
        UpdatedDate = updatedDate;
        RepliedToMessage = repliedToMessage;
    }
}
