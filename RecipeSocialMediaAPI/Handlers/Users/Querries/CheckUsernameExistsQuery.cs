using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Handlers.Users.Querries
{
    public record CheckUsernameExistsQuery(UserDto User) : IRequest<bool>;

    public class CheckUsernameExistsHandler : IRequestHandler<CheckUsernameExistsQuery, bool>
    {
        private readonly IMongoRepository<UserDocument> _userCollection;

        public CheckUsernameExistsHandler(IMongoCollectionFactory factory)
        {
            _userCollection = factory.GetCollection<UserDocument>();
        }

        public Task<bool> Handle(CheckUsernameExistsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userCollection.Contains(x => x.UserName == request.User.UserName));
        }
    }
}
