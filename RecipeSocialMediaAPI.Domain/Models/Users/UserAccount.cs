using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Users;

public class UserAccount : IUserAccount
{
    public string Id { get; }
    public string Handler { get; }
    public string UserName { get; set; }
    public string? ProfileImageId { get; set; }
    public DateTimeOffset AccountCreationDate { get; }
    public UserRole Role { get; set; }

    private readonly HashSet<string> _pinnedConversationIds;
    public ImmutableList<string> PinnedConversationIds => _pinnedConversationIds.ToImmutableList();
    private readonly HashSet<string> _blockedConnectionIds;
    public ImmutableList<string> BlockedConnectionIds => _blockedConnectionIds.ToImmutableList();

    internal UserAccount(
        string id,
        string handler,
        string username,
        string? profileImageId,
        DateTimeOffset accountCreationDate,
        List<string>? pinnedConversationIds = null,
        List<string>? blockedConnectionIds = null,
        UserRole role = UserRole.User)
    {
        Id = id;
        Handler = handler;
        UserName = username;
        ProfileImageId = profileImageId;
        AccountCreationDate = accountCreationDate;
        _pinnedConversationIds = pinnedConversationIds?.ToHashSet() ?? new();
        _blockedConnectionIds = blockedConnectionIds?.ToHashSet() ?? new();
        Role = role;
    }

    public bool RemovePin(string pinnedConversationId) => _pinnedConversationIds.Remove(pinnedConversationId);

    public bool AddPin(string pinnedConversationId) => _pinnedConversationIds.Add(pinnedConversationId);
    public bool UnblockConnection(string connectionId) => _blockedConnectionIds.Remove(connectionId);
    public bool BlockConnection(string connectionId) => _blockedConnectionIds.Add(connectionId);
}
