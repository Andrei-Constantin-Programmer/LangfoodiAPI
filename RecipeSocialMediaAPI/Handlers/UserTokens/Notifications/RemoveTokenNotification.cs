using MediatR;
using MongoDB.Bson;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Endpoints;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Notifications
{
    public record RemoveTokenNotification(string Token) : INotification;

    public class RemoveTokenHandler : INotificationHandler<RemoveTokenNotification>
    {
        private readonly IUserTokenService _userTokenService;
        private readonly IMongoCollectionWrapper<UserTokenDocument> _userTokenCollection;

        public RemoveTokenHandler(IUserTokenService userTokenService, IMongoFactory factory, IConfigManager config)
        {
            _userTokenService = userTokenService;
            _userTokenCollection = factory.GetCollection<UserTokenDocument>(new UserTokenRepository(), config);
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
