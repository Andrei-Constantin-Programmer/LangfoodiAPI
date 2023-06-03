using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

internal record RemoveUserCommand(UserDto User) : IRequest;

internal class RemoveUserHandler : IRequestHandler<RemoveUserCommand>
{
    private readonly IMapper _mapper;
    private readonly IMongoRepository<UserDocument> _userCollection;

    public RemoveUserHandler(IMapper mapper, IMongoCollectionFactory collectionFactory)
    {
        _mapper = mapper;
        _userCollection = collectionFactory.GetCollection<UserDocument>();
    }

    public Task Handle(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        UserDocument userDoc = _mapper.Map<UserDocument>(request.User);

        var successful = _userCollection.Delete(x => x._id == userDoc._id);

        if (!successful)
        {
            throw new Exception($"Could not remove user with id {userDoc._id}.");
        }

        return Task.CompletedTask;
    }
}
