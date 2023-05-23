using MediatR;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Endpoints;
using RecipeSocialMediaAPI.Handlers.UserTokens.Notifications;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    public record RemoveUserCommand(string Token) : IRequest;

    public class RemoveUserHandler : IRequestHandler<RemoveUserCommand>
    {
        private readonly IPublisher _publisher;
        private readonly IUserTokenService _userTokenService;
        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;

        public RemoveUserHandler(IPublisher publisher, IUserTokenService userTokenService, IMongoFactory factory, IConfigManager config)
        {
            _publisher = publisher;
            _userTokenService = userTokenService;
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
        }

        public async Task Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            if (!_userTokenService.CheckValidToken(request.Token))
            {
                throw new InvalidTokenException();
            }

            UserDocument userDoc = _userTokenService.GetUserFromToken(request.Token);
            await _publisher.Publish(new RemoveTokenNotification(request.Token), cancellationToken);

            var successful = await Task.FromResult(_userCollection.Delete(x => x._id == userDoc._id));

            if (!successful)
            {
                throw new Exception($"Could not remove user with id {userDoc._id}.");
            }

            await Task.CompletedTask;
        }
    }
}
