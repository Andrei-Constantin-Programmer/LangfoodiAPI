using AutoMapper;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserDTO, User>();
        CreateMap<User, UserDTO>();
    }
}
