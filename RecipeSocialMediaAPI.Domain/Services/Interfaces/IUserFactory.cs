using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Services.Interfaces;

public interface IUserFactory
{
    public UserAccount CreateUserAccount(string id, string handler, string username, DateTimeOffset? accountCreationDate = null);

    public UserCredentials CreateUserCredentials (UserAccount userAccount, string email, string password);

    public UserCredentials CreateUserCredentials(string id, string handler, string username, string email, string password, DateTimeOffset? accountCreationDate = null);
}
