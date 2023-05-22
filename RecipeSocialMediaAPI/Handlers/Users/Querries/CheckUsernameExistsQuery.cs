using MediatR;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.Users.Querries
{
    public record CheckUsernameExistsQuery(UserDto User) : IRequest<bool>;

    public class CheckUsernameExistsHandler : IRequestHandler<CheckUsernameExistsQuery, bool>
    {
        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;

        public CheckUsernameExistsHandler(IMongoFactory factory, IConfigManager config)
        {
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
        }

        public Task<bool> Handle(CheckUsernameExistsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userCollection.Contains(x => x.UserName == request.User.UserName));
        }
    }
}
