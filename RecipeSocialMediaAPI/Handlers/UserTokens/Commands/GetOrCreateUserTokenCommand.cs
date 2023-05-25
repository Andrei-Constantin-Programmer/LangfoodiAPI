using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.UserTokens.Notifications;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Commands
{
    internal record GetOrCreateUserTokenCommand(UserDto User) : IRequest<UserTokenDto>;

    internal class GetOrCreateUserTokenHandler : IRequestHandler<GetOrCreateUserTokenCommand, UserTokenDto>
    {
        private readonly IPublisher _publisher;

        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUserTokenService _userTokenService;

        private readonly IMongoRepository<UserDocument> _userCollection;
        private readonly IMongoRepository<UserTokenDocument> _userTokenCollection;

        public GetOrCreateUserTokenHandler(IPublisher publisher, ITokenGeneratorService tokenGeneratorService, IMapper mapper, IUserService userService, IUserTokenService userTokenService, IMongoCollectionFactory collectionFactory)
        {
            _publisher = publisher;
            _tokenGeneratorService = tokenGeneratorService;
            _mapper = mapper;
            _userService = userService;
            _userTokenService = userTokenService;

            _userCollection = collectionFactory.GetCollection<UserDocument>();
            _userTokenCollection = collectionFactory.GetCollection<UserTokenDocument>();
        }

        public async Task<UserTokenDto> Handle(GetOrCreateUserTokenCommand request, CancellationToken cancellationToken)
        {
            if (!_userService.DoesUserExist(request.User))
            {
                throw new UserNotFoundException();
            }

            if (_userTokenService.CheckTokenExists(request.User))
            {
                if (_userTokenService.CheckTokenExpired(request.User))
                {
                    await _publisher.Publish(new RemoveTokenForUserNotification(request.User), cancellationToken);
                }
                
                return _tokenGeneratorService.GenerateToken(request.User);
            }

            UserDocument? userDocument = null;
            if (request.User.Email != string.Empty)
            {
                userDocument = _userCollection.Find(x => x.Email.ToLower() == request.User.Email.ToLower());
            }
            else if (request.User.UserName != string.Empty)
            {
                userDocument = _userCollection.Find(x => x.UserName == request.User.UserName);
            }

            if (userDocument is null)
            {
                throw new UserNotFoundException();
            }

            return await Task.FromResult(_mapper.Map<UserTokenDto>(_userTokenCollection.Find(x => x.UserId == userDocument!._id)));
        }
    }
}
