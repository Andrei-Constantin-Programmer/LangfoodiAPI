using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Users;

public class UserMapper : IUserMapper
{
    public UserDto MapUserToUserDto(IUserCredentials userCredentials)
    {
        return new UserDto(
            Id: userCredentials.Account.Id,
            Handler: userCredentials.Account.Handler,
            UserName: userCredentials.Account.UserName,
            AccountCreationDate: userCredentials.Account.AccountCreationDate,
            Email: userCredentials.Email,
            Password: userCredentials.Password,
            ProfileImageId: userCredentials.Account.ProfileImageId,
            PinnedConversationIds: userCredentials.Account.PinnedConversationIds.ToList(),
            BlockedConnectionIds: userCredentials.Account.BlockedConnectionIds.ToList()
        );
    }

    public UserAccountDto MapUserAccountToUserAccountDto(IUserAccount userAccount)
    {
        return new UserAccountDto(
            Id: userAccount.Id,
            Handler: userAccount.Handler,
            UserName: userAccount.UserName,
            ProfileImageId: userAccount.ProfileImageId,
            AccountCreationDate: userAccount.AccountCreationDate,
            PinnedConversationIds: userAccount.PinnedConversationIds.ToList(),
            BlockedConnectionIds: userAccount.BlockedConnectionIds.ToList()
        );
    }

    public UserPreviewForMessageDto MapUserAccountToUserPreviewForMessageDto(IUserAccount userAccount)
    {
        return new UserPreviewForMessageDto(
            Id: userAccount.Id,
            Username: userAccount.UserName,
            ProfileImageId: userAccount.ProfileImageId
        );
    }
}
