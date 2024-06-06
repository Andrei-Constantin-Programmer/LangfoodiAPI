using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Interfaces;

public interface IUserMapper
{
    UserDto MapUserToUserDto(IUserCredentials userCredentials);
    UserAccountDto MapUserAccountToUserAccountDto(IUserAccount userAccount);
    UserPreviewForMessageDto MapUserAccountToUserPreviewForMessageDto(IUserAccount userAccount);
}
