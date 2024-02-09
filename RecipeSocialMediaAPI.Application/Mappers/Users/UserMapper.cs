using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Users;

public class UserMapper : IUserMapper
{
    public UserDTO MapUserToUserDto(IUserCredentials userCredentials)
    {
        return new UserDTO(
            Id: userCredentials.Account.Id,
            Handler: userCredentials.Account.Handler,
            UserName: userCredentials.Account.UserName,
            AccountCreationDate: userCredentials.Account.AccountCreationDate,
            Email: userCredentials.Email,
            Password: userCredentials.Password,
            ProfileImageId: userCredentials.Account.ProfileImageId,
            PinnedConversationIds: userCredentials.Account.PinnedConversationIds.ToList()
        );
    }

    public UserAccountDTO MapUserAccountToUserAccountDto(IUserAccount userAccount)
    {
        return new UserAccountDTO(
            Id: userAccount.Id,
            Handler: userAccount.Handler,
            UserName: userAccount.UserName,
            ProfileImageId: userAccount.ProfileImageId,
            AccountCreationDate: userAccount.AccountCreationDate,
            PinnedConversationIds: userAccount.PinnedConversationIds.ToList()
        );
    }
}
