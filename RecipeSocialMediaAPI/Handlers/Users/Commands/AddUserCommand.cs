using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    internal record AddUserCommand(UserDto User) : IRequest<UserTokenDto>;

    internal class AddUserHandler : IRequestHandler<AddUserCommand, UserTokenDto>
    {
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly IUserValidationService _userValidationService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        private readonly IMongoRepository<UserDocument> _userCollection;

        public AddUserHandler(ITokenGeneratorService tokenGeneratorService, IUserValidationService userValidationService, IUserService userService, IMapper mapper, IMongoCollectionFactory collectionFactory)
        {
            _tokenGeneratorService = tokenGeneratorService;
            _userValidationService = userValidationService;
            _userService = userService;
            _mapper = mapper;
            _userCollection = collectionFactory.GetCollection<UserDocument>();
        }

        public Task<UserTokenDto> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            if (!_userValidationService.ValidUser(request.User)
                || (!_userService.DoesUsernameExist(request.User.UserName)
                && !_userService.DoesEmailExist(request.User.Email)))
            {
                throw new InvalidCredentialsException();
            }

            request.User.Password = _userValidationService.HashPassword(request.User.Password);
            UserDocument insertedUser = _userCollection.Insert(_mapper.Map<UserDocument>(request.User));

            return Task.FromResult(_tokenGeneratorService.GenerateToken(insertedUser));
        }
    }
}
