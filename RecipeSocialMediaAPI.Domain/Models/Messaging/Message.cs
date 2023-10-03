using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RecipeSocialMediaAPI.Domain.Tests.Unit")]

namespace RecipeSocialMediaAPI.Domain.Models.Messaging;

public abstract record Message
{
    protected readonly IDateTimeProvider _dateTimeProvider;

    public string Id { get; }
    public User Sender { get; }
    public DateTimeOffset SentDate { get; }
    public DateTimeOffset? UpdatedDate { get; protected set; }

    internal Message(IDateTimeProvider dateTimeProvider, 
        string id, User sender, DateTimeOffset sentDate, DateTimeOffset? updatedDate)
    {
        _dateTimeProvider = dateTimeProvider;
        Id = id;
        Sender = sender;
        SentDate = sentDate;
        UpdatedDate = updatedDate;
    }
}
