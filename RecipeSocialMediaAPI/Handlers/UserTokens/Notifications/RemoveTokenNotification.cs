using MediatR;
using MongoDB.Bson;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Notifications
{
    public record RemoveTokenNotification(string Token) : INotification;

    public class RemoveTokenHandler : INotificationHandler<RemoveTokenNotification>
    {
        private readonly IMongoCollectionWrapper<UserTokenDocument> _userTokenCollection;

        public RemoveTokenHandler(IMongoFactory factory, IConfigManager config)
        {
            _userTokenCollection = factory.GetCollection<UserTokenDocument>(new UserTokenRepository(), config);
        }

        public Task Handle(RemoveTokenNotification notification, CancellationToken cancellationToken)
        {
            ObjectId tokenObj = ObjectId.Parse(notification.Token);
            _userTokenCollection.Delete(x => x._id == tokenObj);

            return Task.CompletedTask;
        }
    }
}
