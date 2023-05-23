using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Utilities;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;

        public UserService(IMongoFactory factory, IConfigManager config)
        {
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
        }

        public bool DoesUserExist(UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.Email.ToLower() == user.Email.ToLower());
            }
            else if (user.UserName != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.UserName == user.UserName);
            }

            return userDoc != null && BCrypter.Verify(user.Password, userDoc.Password);
        }
    }
}
