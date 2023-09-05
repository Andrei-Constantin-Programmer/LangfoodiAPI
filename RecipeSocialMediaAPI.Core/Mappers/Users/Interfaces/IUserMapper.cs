using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
public interface IUserMapper
{
    public UserDTO MapUserToUserDto(User user);
    public User MapUserDtoToUser(UserDTO userDto);
}
