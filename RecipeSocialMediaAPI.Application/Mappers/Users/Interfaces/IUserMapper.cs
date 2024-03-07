using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Interfaces;

public interface IUserMapper
{
    UserDTO MapUserToUserDto(IUserCredentials userCredentials);
    UserAccountDTO MapUserAccountToUserAccountDto(IUserAccount userAccount);
    UserPreviewForMessageDTO MapUserAccountToUserPreviewForMessageDto(IUserAccount userAccount);
}
