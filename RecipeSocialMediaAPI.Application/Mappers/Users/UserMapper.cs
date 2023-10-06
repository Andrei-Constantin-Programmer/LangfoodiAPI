using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Users;
public class UserMapper : IUserMapper
{
    public UserDTO MapUserToUserDto(UserCredentials user)
    {
        return new UserDTO()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Password = user.Password,
        };
    }

    public UserCredentials MapUserDtoToUser(UserDTO userDto)
    {
        return new(userDto.Id, userDto.UserName, userDto.Email, userDto.Password);
    }
}
