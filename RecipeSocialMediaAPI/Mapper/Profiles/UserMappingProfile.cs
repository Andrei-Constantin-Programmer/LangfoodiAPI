using AutoMapper;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Mapper.Profiles
{
    internal class UserMappingProfile : Profile
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
