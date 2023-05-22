using MediatR;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.UserTokens.Commands
{
    public record GenerateTokenCommand(UserDto User) : IRequest<UserTokenDto>;

    public class GenerateTokenHandler : IRequestHandler<GenerateTokenCommand, UserTokenDto>
    {
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;

        public GenerateTokenHandler(ITokenGeneratorService tokenGeneratorService, IMongoFactory factory, IConfigManager config)
        {
            _tokenGeneratorService = tokenGeneratorService;
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
        }

        public Task<UserTokenDto> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
        {
            UserDocument? userDoc = null;
            if (request.User.Email != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.Email.ToLower() == request.User.Email.ToLower());
            }
            else if (request.User.UserName != string.Empty)
            {
                userDoc = _userCollection.Find(x => x.UserName == request.User.UserName);
            }
            if(userDoc is null)
            {
                throw new UserNotFoundException(request.User);
            }

            return Task.FromResult(_tokenGeneratorService.GenerateToken(userDoc));
        }
    }
}
