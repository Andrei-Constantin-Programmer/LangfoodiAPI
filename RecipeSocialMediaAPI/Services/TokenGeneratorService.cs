using AutoMapper;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services.Interfaces;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Services
{
    public class TokenGeneratorService : ITokenGeneratorService
    {
        private readonly IClock _clock;
        private readonly IMapper _mapper;
        private readonly IMongoCollectionWrapper<UserTokenDocument> _userTokenCollection;

        public TokenGeneratorService(IClock clock, IMapper mapper, IMongoFactory factory, IConfigManager config)
        {
            _clock = clock;
            _mapper = mapper;
            _userTokenCollection = factory.GetCollection<UserTokenDocument>(new UserTokenRepository(), config);
        }

        public UserTokenDto GenerateToken(UserDocument user)
        {
            UserTokenDocument tokenToInsert = new()
            {
                UserId = user._id,
                ExpiryDate = _clock.Now.AddMonths(3),
            };
            _userTokenCollection.Insert(tokenToInsert);

            return _mapper.Map<UserTokenDto>(tokenToInsert);
        }
    }
}
