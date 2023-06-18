using AutoMapper;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Mapper.Profiles;

internal class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserDTO, UserDocument>()
            .ForMember(dest => dest.Id, config => config.MapFrom(src => src.Id));
        CreateMap<UserDocument, UserDTO>()
            .ForMember(dest => dest.Id, config => config.MapFrom(src => src.Id));

        CreateMap<UserDocument, NewUserDTO>();
        CreateMap<NewUserDTO, UserDocument>();
    }
}
