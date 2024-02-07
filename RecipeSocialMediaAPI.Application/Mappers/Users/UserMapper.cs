using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.Application.Mappers.Users;

public class UserMapper : IUserMapper
{
    private readonly IUserFactory _userFactory;

    public UserMapper(IUserFactory userFactory)
    {
        _userFactory = userFactory;
    }

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

    public IUserCredentials MapUserDtoToUser(UserDTO userDto)
    {
        return _userFactory.CreateUserCredentials(
            userDto.Id, 
            userDto.Handler, 
            userDto.UserName, 
            userDto.Email, 
            userDto.Password,
            userDto.ProfileImageId,
            userDto.AccountCreationDate);
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

    public IUserAccount MapUserAccountDtoToUserAccount(UserAccountDTO userAccountDto)
    {
        return _userFactory.CreateUserAccount(
            userAccountDto.Id,
            userAccountDto.Handler,
            userAccountDto.UserName,
            userAccountDto.ProfileImageId,
            userAccountDto.AccountCreationDate);
    }
}
