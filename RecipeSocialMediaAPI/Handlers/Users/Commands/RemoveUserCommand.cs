using MediatR;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Handlers.UserTokens.Notifications;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    public record RemoveUserCommand(string Token) : IRequest<bool>;

    public class RemoveUserHandler : IRequestHandler<RemoveUserCommand, bool>
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

        public async Task<bool> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            UserDocument userDoc = _userTokenService.GetUserFromToken(request.Token);
            await _publisher.Publish(new RemoveTokenNotification(request.Token), cancellationToken);

            return await Task.FromResult(_userCollection.Delete(x => x._id == userDoc._id));
        }
    }
}
