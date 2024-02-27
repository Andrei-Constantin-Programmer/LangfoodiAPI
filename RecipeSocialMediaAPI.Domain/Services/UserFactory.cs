using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Domain.Services;

public class UserFactory : IUserFactory
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserFactory(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public IUserAccount CreateUserAccount(
        string id,
        string handler,
        string username,
        string? profileImageId = null,
        DateTimeOffset? accountCreationDate = null,
        List<string>? pinnedConversationIds = null,
        List<string>? blockedConnectionIds = null,
        UserRole userRole = UserRole.User)
    {
        return new UserAccount(id, handler, username, profileImageId, accountCreationDate ?? _dateTimeProvider.Now, pinnedConversationIds, blockedConnectionIds, userRole);
    }

    public IUserCredentials CreateUserCredentials(IUserAccount userAccount, string email, string password)
    {
        return new UserCredentials(userAccount, email, password);
    }

    public IUserCredentials CreateUserCredentials(
        string id,
        string handler,
        string username,
        string email,
        string password,
        string? profileImageId = null,
        DateTimeOffset? accountCreationDate = null,
        List<string>? pinnedConversationIds = null,
        List<string>? blockedConnectionIds = null,
        UserRole userRole = UserRole.User)
    {
        return new UserCredentials(CreateUserAccount(id, handler, username, profileImageId, accountCreationDate, pinnedConversationIds, blockedConnectionIds, userRole), email, password);
    }
}
