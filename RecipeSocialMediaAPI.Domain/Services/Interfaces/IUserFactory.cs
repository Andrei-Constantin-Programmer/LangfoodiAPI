using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Domain.Services.Interfaces;

public interface IUserFactory
{
    [ExcludeFromCodeCoverage(Justification = "Implementation is tested instead")]
    IUserAccount CreateUserAccount(
        string id,
        string handler,
        string username,
        string? profileImageId = null,
        DateTimeOffset? accountCreationDate = null,
        List<string>? pinnedConversationIds = null,
        List<string>? blockedConnectionIds = null,
        UserRole userRole = UserRole.User);

    [ExcludeFromCodeCoverage(Justification = "Implementation is tested instead")]
    IUserCredentials CreateUserCredentials(
        string id,
        string handler,
        string username,
        string email,
        string password,
        string? profileImageId = null,
        DateTimeOffset? accountCreationDate = null,
        List<string>? pinnedConversationIds = null,
        List<string>? blockedConnectionIds = null,
        UserRole userRole = UserRole.User);
}
