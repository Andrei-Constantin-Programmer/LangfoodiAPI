using MediatR;
using MongoDB.Bson;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Endpoints;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Notifications
{
    internal record RemoveTokenNotification(string Token) : INotification;

    internal class RemoveTokenHandler : INotificationHandler<RemoveTokenNotification>
    {
        private readonly IUserTokenService _userTokenService;
        private readonly IMongoRepository<UserTokenDocument> _userTokenCollection;

        public RemoveTokenHandler(IUserTokenService userTokenService, IMongoCollectionFactory collectionFactory)
        {
            _userTokenService = userTokenService;
            _userTokenCollection = collectionFactory.GetCollection<UserTokenDocument>();
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
