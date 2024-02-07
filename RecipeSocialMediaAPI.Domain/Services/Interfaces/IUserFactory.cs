using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Services.Interfaces;

public interface IUserFactory
{
    public IUserAccount CreateUserAccount(string id, string handler, string username, string? profileImageId = null, DateTimeOffset? accountCreationDate = null, List<string>? pinnedConversationIds = null);

    public IUserCredentials CreateUserCredentials (IUserAccount userAccount, string email, string password);

    public IUserCredentials CreateUserCredentials(string id, string handler, string username, string email, string password, string? profileImageId = null, DateTimeOffset? accountCreationDate = null, List<string>? pinnedConversationIds = null);
}
