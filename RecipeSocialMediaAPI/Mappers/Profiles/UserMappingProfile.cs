using AutoMapper;
using RecipeSocialMediaAPI.Contracts;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DTO;

namespace RecipeSocialMediaAPI.Mappers.Profiles;

internal class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserDTO, UserDocument>()
            .ForMember(dest => dest.Id, config => config.MapFrom(src => src.Id));
        CreateMap<UserDocument, UserDTO>()
            .ForMember(dest => dest.Id, config => config.MapFrom(src => src.Id));

        CreateMap<UserDocument, NewUserContract>();
        CreateMap<NewUserContract, UserDocument>();
    }
}
