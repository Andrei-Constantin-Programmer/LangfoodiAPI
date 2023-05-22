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
        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;

        public UserService(IMapper mapper, IMongoFactory factory, IConfigManager config) {
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
            _mapper = mapper;
        }

        public bool UpdateUser(IUserValidationService validationService, IUserTokenService userTokenService, string token, UserDto user)
        {
            user.Password = validationService.HashPassword(user.Password);
            UserDocument newUserDoc = _mapper.Map<UserDocument>(user);
            newUserDoc._id = userTokenService.GetUserFromToken(token)._id;

            return _userCollection.UpdateRecord(newUserDoc, x => x._id == newUserDoc._id);
        }
    }
}
