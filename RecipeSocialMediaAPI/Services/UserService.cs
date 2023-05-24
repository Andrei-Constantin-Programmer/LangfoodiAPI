using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.Data.DTO;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoRepository<UserDocument> _userCollection;

        public UserService(IMongoCollectionFactory factory)
        {
            _userCollection = factory.GetCollection<UserDocument>();
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
