using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;

namespace RecipeSocialMediaAPI.Handlers.Users.Queries;

internal record CheckUsernameExistsQuery(string Username) : IRequest<bool>;

internal class CheckUsernameExistsHandler : IRequestHandler<CheckUsernameExistsQuery, bool>
{
    private readonly IMongoRepository<UserDocument> _userCollection;

    public CheckUsernameExistsHandler(IMongoCollectionFactory collectionFactory)
    {
        _userCollection = collectionFactory.GetCollection<UserDocument>();
    }

    public Task<bool> Handle(CheckUsernameExistsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_userCollection.Contains(x => x.UserName == request.Username));
    }
}
