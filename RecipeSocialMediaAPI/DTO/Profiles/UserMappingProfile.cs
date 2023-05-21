using AutoMapper;
using RecipeSocialMediaAPI.DTO.Mongo;

namespace RecipeSocialMediaAPI.DTO.Profiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<UserDto, UserDocument>();
            CreateMap<UserDocument, UserDto>()
                .ForSourceMember(s => s._id, o => o.DoNotValidate())
                .ForSourceMember(s => s.Password, o => o.DoNotValidate());
        }
    }
}
