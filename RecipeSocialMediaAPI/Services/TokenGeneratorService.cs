using AutoMapper;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Services.Interfaces;

namespace RecipeSocialMediaAPI.Services
{
    internal class TokenGeneratorService : ITokenGeneratorService
    {
        private readonly IClock _clock;
        private readonly IMapper _mapper;
        private readonly IMongoRepository<UserDocument> _userCollection;
        private readonly IMongoRepository<UserTokenDocument> _userTokenCollection;

        public TokenGeneratorService(IClock clock, IMapper mapper, IMongoCollectionFactory collectionFactory)
        {
            _clock = clock;
            _mapper = mapper;
            _userTokenCollection = collectionFactory.GetCollection<UserTokenDocument>();
            _userCollection = collectionFactory.GetCollection<UserDocument>();
        }

        public UserTokenDto GenerateToken(UserDto user)
        {
            UserDocument? userDocument = null;
            if(user.Email != string.Empty)
            {
                userDocument = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            }
            else if (user.UserName != string.Empty)
            {
                userDocument = _userCollection.Find(x => x.UserName.ToLower() == user.UserName.ToLower());
            }

            if(userDocument is null)
            {
                throw new UserNotFoundException();
            }

            return GenerateToken(userDocument);
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
