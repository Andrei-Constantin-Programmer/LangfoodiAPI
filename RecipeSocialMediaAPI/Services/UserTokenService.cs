using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DTO.Mongo;
using RecipeSocialMediaAPI.Utilities;
using MongoDB.Bson;
using RecipeSocialMediaAPI.DTO;
using AutoMapper;

namespace RecipeSocialMediaAPI.Services
{
    public class UserTokenService : IUserTokenService
    {
        private readonly IMongoCollectionManager<UserTokenDocument> _userTokenCollection;
        private readonly IMongoCollectionManager<UserDocument> _userCollection;
        private readonly IClock _clock;
        private readonly IMapper _mapper;

        public UserTokenService(IMapper mapper, IMongoFactory factory, IConfigManager config, IClock clock)
        {
            _userTokenCollection = factory.GetCollectionManager<UserTokenDocument>(new UserTokenRepository(), config);
            _userCollection = factory.GetCollectionManager<UserDocument>(new UserRepository(), config);
            _clock = clock;
            _mapper = mapper;
        }

        #region Write Methods
        public bool RemoveToken(UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty) userDoc = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            else if (user.UserName != string.Empty) userDoc = _userCollection.Find(x => x.UserName == user.UserName);

            return _userTokenCollection.Delete(x => x.UserId == userDoc!._id);
        }
        
        public bool RemoveToken(string token)
        {
            ObjectId tokenObj = ObjectId.Parse(token);
            return _userTokenCollection.Delete(x => x._id == tokenObj);
        }

        public UserTokenDto GenerateToken(UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty) userDoc = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            else if (user.UserName != string.Empty) userDoc = _userCollection.Find(x => x.UserName == user.UserName);

            return GenerateToken(userDoc!);
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
        #endregion

        #region Read Methods
        public UserDocument GetUserFromTokenWithPassword(string token)
        {
            ObjectId tokenObj = ObjectId.Parse(token);
            UserTokenDocument userToken = _userTokenCollection.Find(x => x._id == tokenObj)!;

            return _userCollection.Find(x => x._id == userToken.UserId)!;
        }

        public UserDocument GetUserFromToken(string token)
        {
            UserDocument user = GetUserFromTokenWithPassword(token);
            user.Password = "";
            return user;
        }

        public UserTokenDto GetTokenFromUser(UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty) userDoc = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            else if (user.UserName != string.Empty) userDoc = _userCollection.Find(x => x.UserName == user.UserName);

            return _mapper.Map<UserTokenDto>(_userTokenCollection.Find(x => x.UserId == userDoc!._id));
        }
        #endregion

        #region Validation Methods
        public bool CheckValidToken(string token)
        {
            return CheckTokenExists(token) && !CheckTokenExpired(token);
        }

        public bool CheckTokenExists(UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty) userDoc = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            else if (user.UserName != string.Empty) userDoc = _userCollection.Find(x => x.UserName == user.UserName);

            return _userTokenCollection.Contains(x => x.UserId == userDoc!._id);
        }

        public bool CheckTokenExpired(UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty) userDoc = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            else if (user.UserName != string.Empty) userDoc = _userCollection.Find(x => x.UserName == user.UserName);

            UserTokenDocument tokenDoc = _userTokenCollection.Find(x => x.UserId == userDoc!._id)!;
            return _clock.Now >= tokenDoc.ExpiryDate;
        }

        public bool CheckTokenExpired(string token)
        {
            ObjectId tokenObj = ObjectId.Parse(token);
            UserTokenDocument tokenDoc = _userTokenCollection.Find(x => x._id! == tokenObj)!;
            return _clock.Now >= tokenDoc.ExpiryDate;
        }

        public bool CheckTokenExists(string token)
        {
            ObjectId tokenObj = ObjectId.Parse(token);
            return _userTokenCollection.Contains(x => x._id! == tokenObj);
        }
        #endregion
    }
}
