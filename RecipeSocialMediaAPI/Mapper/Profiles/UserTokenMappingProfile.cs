using AutoMapper;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Mapper.Profiles
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
