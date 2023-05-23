using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.UserTokens.Notifications;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Commands
{
    public record GetOrCreateUserTokenCommand(UserDto User) : IRequest<UserTokenDto>;

    public class GetOrCreateUserTokenHandler : IRequestHandler<GetOrCreateUserTokenCommand, UserTokenDto>
    {
        private readonly IPublisher _publisher;

        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUserTokenService _userTokenService;

        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;
        private readonly IMongoCollectionWrapper<UserTokenDocument> _userTokenCollection;

        public GetOrCreateUserTokenHandler(IPublisher publisher, ITokenGeneratorService tokenGeneratorService, IMapper mapper, IUserService userService, IUserTokenService userTokenService, IMongoFactory factory, IConfigManager config)
        {
            _publisher = publisher;
            _tokenGeneratorService = tokenGeneratorService;
            _mapper = mapper;
            _userService = userService;
            _userTokenService = userTokenService;

            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
            _userTokenCollection = factory.GetCollection<UserTokenDocument>(new UserTokenRepository(), config);
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
