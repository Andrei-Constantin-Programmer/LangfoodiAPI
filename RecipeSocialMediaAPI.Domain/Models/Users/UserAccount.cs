using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Users;

public class UserAccount : IUserAccount
{
    public string Id { get; }
    public string Handler { get; }
    public string UserName { get; set; }
    public string? ProfileImageId { get; set; }
    public DateTimeOffset AccountCreationDate { get; }

    private readonly HashSet<Conversation> _pinnedConversations;
    public ImmutableList<Conversation> PinnedConversations => _pinnedConversations.ToImmutableList();

    internal UserAccount(string id, string handler, string username, string? profileImageId, DateTimeOffset accountCreationDate, List<Conversation>? pinnedConversations = null)
    {
        Id = id;
        Handler = handler;
        UserName = username;
        ProfileImageId = profileImageId;
        AccountCreationDate = accountCreationDate;
        _pinnedConversations = pinnedConversations?.ToHashSet() ?? new();
    }

    public bool RemovePin(Conversation pinnedConversation) => _pinnedConversations.Remove(pinnedConversation);

    public bool AddPin(Conversation pinnedConversation) => _pinnedConversations.Add(pinnedConversation);
}
