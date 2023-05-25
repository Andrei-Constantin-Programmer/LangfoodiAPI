using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Services
{
    internal class UserService : IUserService
    {
        private readonly IMongoRepository<UserDocument> _userCollection;

        public UserService(IMongoCollectionFactory collectionFactory)
        {
            _userCollection = collectionFactory.GetCollection<UserDocument>();
        }

        public bool DoesUserExist(UserDto user)
        {
            UserDocument? userDoc = null;
            if (user.Email != string.Empty)
            {
                userDoc = _userCollection.Find(user => user.Email.ToLower() == user.Email.ToLower());
            }
            else if (user.UserName != string.Empty)
            {
                userDoc = _userCollection.Find(user => user.UserName == user.UserName);
            }

            return userDoc != null && BCrypter.Verify(user.Password, userDoc.Password);
        }

        public bool DoesEmailExist(string email) =>
            _userCollection
                .Contains(user => user.Email.ToLower() == email.ToLower());

        public bool DoesUsernameExist(string username) =>
            _userCollection
                .Contains(user => user.UserName == username);

    }
}
