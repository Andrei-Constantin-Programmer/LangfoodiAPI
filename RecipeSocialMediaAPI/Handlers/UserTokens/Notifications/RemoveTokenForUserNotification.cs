using Amazon.Runtime.Internal;
using MediatR;
using MongoDB.Bson;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Notifications
{
    public record RemoveTokenForUserNotification(UserDto User) : INotification;

    public class RemoveTokenForUserHandler : INotificationHandler<RemoveTokenForUserNotification>
    {
        private readonly IMongoRepository<UserTokenDocument> _userTokenCollection;
        private readonly IMongoRepository<UserDocument> _userCollection;

        public RemoveTokenForUserHandler(IMongoCollectionFactory factory)
        {
            _userTokenCollection = factory.GetCollection<UserTokenDocument>();
            _userCollection = factory.GetCollection<UserDocument>();
        }

        public Task Handle(RemoveTokenForUserNotification notification, CancellationToken cancellationToken)
        {
            UserDocument? userDoc = null;
            if (notification.User.Email != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.Email.ToLower() == notification.User.Email.ToLower());
            }
            else if (notification.User.UserName != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.UserName == notification.User.UserName);
            }

            _userTokenCollection.Delete(x => x.UserId == userDoc!._id);
            
            return Task.CompletedTask;
        }
    }
}
