using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.DTO.Mongo;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Services
{
    public static class UserService
    {
        public static bool CheckUserNameExists(UserDTO user)
        {
            IMongoCollectionManager<UserMDTO> userCollection = new MongoCollectionManager<UserMDTO>(new UserRepository(), new ConfigManager());
            return userCollection.QueryCollection(x => x.UserName.ToLower().Equals(user.UserName.ToLower())).Any();
        }
    }
}
