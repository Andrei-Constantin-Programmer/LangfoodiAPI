using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
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

    public IUserAccount CreateUserAccount(string id, string handler, string username, string? profileImageId = null, DateTimeOffset? accountCreationDate = null, List<Conversation>? pinnedConversations = null)
    {
        return new UserAccount(id, handler, username, profileImageId, accountCreationDate ?? _dateTimeProvider.Now, pinnedConversations);
    }

    public IUserCredentials CreateUserCredentials(IUserAccount userAccount, string email, string password)
    {
        return new UserCredentials(userAccount, email, password);
    }

    public IUserCredentials CreateUserCredentials(string id, string handler, string username, string email, string password, string? profileImageId = null, DateTimeOffset? accountCreationDate = null, List<Conversation>? pinnedConversations = null)
    {
        return new UserCredentials(CreateUserAccount(id, handler, username, profileImageId, accountCreationDate, pinnedConversations), email, password);
    }
}
