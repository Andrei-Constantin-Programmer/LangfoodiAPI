using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Mappers.Users;
public class UserMapper : IUserMapper
{
    public UserDTO MapUserToUserDto(User user)
    {
        return new UserDTO()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Password = user.Password,
        };
    }

    public User MapUserDtoToUser(UserDTO userDto)
    {
        return new(userDto.Id, userDto.UserName, userDto.Email, userDto.Password);
    }
}
