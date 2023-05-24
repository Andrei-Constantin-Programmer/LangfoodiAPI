using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;

using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Handlers.Users.Querries
{
    public record CheckEmailExistsQuery(UserDto User) : IRequest<bool>;

    public class CheckEmailExistsHandler : IRequestHandler<CheckEmailExistsQuery, bool>
    {
        private readonly IMongoRepository<UserDocument> _userCollection;

        public CheckEmailExistsHandler(IMongoCollectionFactory factory)
        {
            _userCollection = factory.GetCollection<UserDocument>();
        }

        public Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userCollection
                .Contains(x => x.Email.ToLower() == request.User.Email.ToLower()));
        }
    }
}
