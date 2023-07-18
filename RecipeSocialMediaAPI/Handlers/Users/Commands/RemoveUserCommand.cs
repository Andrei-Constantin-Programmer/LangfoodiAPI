using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Exceptions;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

public record RemoveUserCommand(string EmailOrId) : IRequest;

internal class RemoveUserHandler : IRequestHandler<RemoveUserCommand>
{
    private readonly IMongoRepository<UserDocument> _userCollection;

    public RemoveUserHandler(IMongoCollectionFactory collectionFactory)
    {
        _userCollection = collectionFactory.GetCollection<UserDocument>();
    }

    public Task Handle(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        UserDocument userDoc = _userCollection.Find(user => user.Email == request.EmailOrId)
                            ?? _userCollection.Find(user => user.Id == request.EmailOrId)
                            ?? throw new UserNotFoundException();

        var successful = _userCollection.Delete(x => x.Id == userDoc.Id);

        return successful 
            ? Task.CompletedTask 
            : throw new Exception($"Could not remove user with id {userDoc.Id}.");
    }
}
