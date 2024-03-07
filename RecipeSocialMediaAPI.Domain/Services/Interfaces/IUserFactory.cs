using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Services.Interfaces;

public interface IUserFactory
{
    IUserAccount CreateUserAccount(string id, string handler, string username, string? profileImageId = null, DateTimeOffset? accountCreationDate = null, List<string>? pinnedConversationIds = null, List<string>? blockedConnectionIds = null, UserRole userRole = UserRole.User);

    IUserCredentials CreateUserCredentials(string id, string handler, string username, string email, string password, string? profileImageId = null, DateTimeOffset? accountCreationDate = null, List<string>? pinnedConversationIds = null, List<string>? blockedConnectionIds = null, UserRole userRole = UserRole.User);
}
