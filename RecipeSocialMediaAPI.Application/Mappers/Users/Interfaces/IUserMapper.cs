using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Interfaces;
public interface IUserMapper
{
    public UserDTO MapUserToUserDto(IUserCredentials userCredentials);
    public IUserCredentials MapUserDtoToUser(UserDTO userDto);
    public UserAccountDTO MapUserAccountToUserAccountDto(IUserAccount userAccount);
    public IUserAccount MapUserAccountDtoToUserAccount(UserAccountDTO userAccountDto);
}
