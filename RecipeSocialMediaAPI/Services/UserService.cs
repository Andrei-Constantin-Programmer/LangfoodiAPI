using AutoMapper;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollectionManager<UserDocument> _userCollection;

        public UserService(IMapper mapper, IMongoFactory factory, IConfigManager config) {
            _userCollection = factory.GetCollectionManager<UserDocument>(new UserRepository(), config);
            _mapper = mapper;
        }

        public bool ValidUserLogin(IValidationService validationService, UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty) userDoc = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            else if (user.UserName != string.Empty) userDoc = _userCollection.Find(x => x.UserName == user.UserName);
            return userDoc != null && BCrypt.Net.BCrypt.Verify(user.Password, userDoc.Password);
        }

        public bool CheckEmailExists(UserDto user)
        {
            return _userCollection.Contains(x => x.Email.ToLower() == user.Email.ToLower());
        }

        public bool CheckUserNameExists(UserDto user)
        {
            return _userCollection.Contains(x => x.UserName == user.UserName);
        }

        public UserTokenDto AddUser(UserDto user, IUserTokenService userTokenService, IValidationService validationService)
        {
            user.Password = validationService.HashPassword(user.Password);
            UserDocument insertedUser = _userCollection.Insert(_mapper.Map<UserDocument>(user));
            return userTokenService.GenerateToken(insertedUser);
        }

        public bool RemoveUser(string token, IUserTokenService userTokenService)
        {
            UserDocument userDoc = userTokenService.GetUserFromToken(token);
            userTokenService.RemoveToken(token);
            return _userCollection.Delete(x => x._id == userDoc._id);
        }

        public bool UpdateUser(IValidationService validationService, IUserTokenService userTokenService, string token, UserDto user)
        {
            user.Password = validationService.HashPassword(user.Password);
            UserDocument newUserDoc = _mapper.Map<UserDocument>(user);
            newUserDoc._id = userTokenService.GetUserFromToken(token)._id;

            return _userCollection.UpdateRecord(newUserDoc, x => x._id == newUserDoc._id);
        }
    }
}
