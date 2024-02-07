using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Tests.Shared;

public class TestUserAccount : IUserAccount
{
    required public string Id { get; set; }
    required public string Handler { get; set; }
    required public string UserName { get; set; }
    public string? ProfileImageId { get; set; }
    public DateTimeOffset AccountCreationDate { get; set; }

    private readonly HashSet<string> _pinnedConversationIds = new();
    public ImmutableList<string> PinnedConversationIds => _pinnedConversationIds.ToImmutableList();

    public bool AddPin(string pinnedConversationId) => _pinnedConversationIds.Add(pinnedConversationId);
    public bool RemovePin(string pinnedConversationId) => _pinnedConversationIds.Remove(pinnedConversationId);
}