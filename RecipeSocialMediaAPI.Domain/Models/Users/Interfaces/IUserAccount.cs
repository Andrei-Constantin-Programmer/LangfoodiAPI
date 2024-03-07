using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Users;

public interface IUserAccount
{
    string Id { get; }
    string Handler { get; }
    string UserName { get; set; }
    string? ProfileImageId { get; set; }
    DateTimeOffset AccountCreationDate { get; }
    ImmutableList<string> PinnedConversationIds { get; }
    ImmutableList<string> BlockedConnectionIds { get; }
    UserRole Role { get; set; }

    bool RemovePin(string pinnedConversationId);
    bool AddPin(string pinnedConversationId);
    bool UnblockConnection(string connectionId);
    bool BlockConnection(string connectionId);
}