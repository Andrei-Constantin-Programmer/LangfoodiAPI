using MediatR;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    public record RemoveUserCommand(string Token) : IRequest<bool>;

    public class RemoveUserHandler : IRequestHandler<RemoveUserCommand, bool>
    {
        private readonly IUserTokenService _userTokenService;
        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;

        public RemoveUserHandler(IUserTokenService userTokenService, IMongoFactory factory, IConfigManager config)
        {
            _userTokenService = userTokenService;
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
        }

        public Task<bool> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            UserDocument userDoc = _userTokenService.GetUserFromToken(request.Token);
            _userTokenService.RemoveToken(request.Token);

            return Task.FromResult(_userCollection.Delete(x => x._id == userDoc._id));
        }
    }
}
