using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Interfaces;

public interface IUserMapper
{
    public UserDTO MapUserToUserDto(IUserCredentials userCredentials);
    public UserAccountDTO MapUserAccountToUserAccountDto(IUserAccount userAccount);
}
