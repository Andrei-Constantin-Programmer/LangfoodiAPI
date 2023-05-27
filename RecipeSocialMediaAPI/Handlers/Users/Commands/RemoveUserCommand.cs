using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Handlers.UserTokens.Notifications;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    internal record RemoveUserCommand(string Token) : IRequest;

    internal class RemoveUserHandler : IRequestHandler<RemoveUserCommand>
    {
        private readonly IPublisher _publisher;
        private readonly IUserTokenService _userTokenService;
        private readonly IMongoRepository<UserDocument> _userCollection;

        public RemoveUserHandler(IPublisher publisher, IUserTokenService userTokenService, IMongoCollectionFactory collectionFactory)
        {
            _publisher = publisher;
            _userTokenService = userTokenService;
            _userCollection = collectionFactory.GetCollection<UserDocument>();
        }

        public async Task Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new RemoveTokenNotification(request.Token), cancellationToken);

            UserDocument userDoc = _userTokenService.GetUserFromToken(request.Token);

            var successful = await Task.FromResult(_userCollection.Delete(x => x._id == userDoc._id));

            if (!successful)
            {
                throw new Exception($"Could not remove user with id {userDoc._id}.");
            }

            await Task.CompletedTask;
        }
    }
}
