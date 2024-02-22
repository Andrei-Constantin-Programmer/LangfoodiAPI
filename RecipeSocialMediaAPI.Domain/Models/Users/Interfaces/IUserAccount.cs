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
    public UserRole Role { get; set; }

    public bool RemovePin(string pinnedConversationId);
    public bool AddPin(string pinnedConversationId);
}