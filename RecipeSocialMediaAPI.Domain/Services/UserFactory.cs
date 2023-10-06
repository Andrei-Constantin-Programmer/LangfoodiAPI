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

    public UserAccount CreateUserAccount(string id, string handler, string username, DateTimeOffset? accountCreationDate = null)
    {
        return new UserAccount(_dateTimeProvider, id, handler, username, accountCreationDate);
    }

    public UserCredentials CreateUserCredentials(UserAccount userAccount, string email, string password)
    {
        return new UserCredentials(userAccount, email, password);
    }

    public UserCredentials CreateUserCredentials(string id, string handler, string username, string email, string password, DateTimeOffset? accountCreationDate = null)
    {
        return new UserCredentials(CreateUserAccount(id, handler, username, accountCreationDate), email, password);
    }
}
