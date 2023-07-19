using AutoMapper;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Model;

namespace RecipeSocialMediaAPI.Mappers.Profiles;

internal class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserDTO, User>();
        CreateMap<User, UserDTO>();
    }
}
