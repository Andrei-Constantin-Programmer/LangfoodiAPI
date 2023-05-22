using AutoMapper;
using RecipeSocialMediaAPI.DTO.Mongo;

namespace RecipeSocialMediaAPI.DTO.Profiles
{
    public class UserTokenMappingProfile : Profile
    {
        public UserTokenMappingProfile() 
        {
            CreateMap<UserTokenDocument, UserTokenDto>()
                .ForMember(d => d.Token, o => o.MapFrom(s => s.TokenId!.ToString()))
                .ForSourceMember(s => s.UserId, o => o.DoNotValidate())
                .ForSourceMember(s => s.ExpiryDate, o => o.DoNotValidate());
        }
    }
}
