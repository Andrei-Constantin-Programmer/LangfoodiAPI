using MediatR;
using MongoDB.Bson;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.Endpoints;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Notifications
{
    public record RemoveTokenNotification(string Token) : INotification;

    public class RemoveTokenHandler : INotificationHandler<RemoveTokenNotification>
    {
        private readonly IUserTokenService _userTokenService;
        private readonly IMongoRepository<UserTokenDocument> _userTokenCollection;

        public RemoveTokenHandler(IUserTokenService userTokenService, IMongoCollectionFactory factory)
        {
            _userTokenService = userTokenService;
            _userTokenCollection = factory.GetCollection<UserTokenDocument>();
        }

        public Task Handle(RemoveTokenNotification notification, CancellationToken cancellationToken)
        {
            if (!_userTokenService.CheckTokenExistsAndNotExpired(notification.Token))
            {
                throw new TokenNotFoundOrExpiredException();
            }

            ObjectId tokenObj = ObjectId.Parse(notification.Token);
            _userTokenCollection.Delete(x => x._id == tokenObj);

            return Task.CompletedTask;
        }
    }
}
