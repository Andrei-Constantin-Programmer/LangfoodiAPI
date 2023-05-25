using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Handlers.Users.Querries
{
    internal record CheckEmailExistsQuery(UserDto User) : IRequest<bool>;

    internal class CheckEmailExistsHandler : IRequestHandler<CheckEmailExistsQuery, bool>
    {
        private readonly IMongoRepository<UserDocument> _userCollection;

        public CheckEmailExistsHandler(IMongoCollectionFactory collectionFactory)
        {
            _userCollection = collectionFactory.GetCollection<UserDocument>();
        }

        public Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userCollection
                .Contains(x => x.Email.ToLower() == request.User.Email.ToLower()));
        }
    }
}
