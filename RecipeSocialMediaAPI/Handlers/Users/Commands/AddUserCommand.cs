using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    internal record AddUserCommand(UserDto User) : IRequest<UserTokenDto>;

    internal class AddUserHandler : IRequestHandler<AddUserCommand, UserTokenDto>
    {
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly IUserValidationService _userValidationService;
        private readonly IMapper _mapper;

        private readonly IMongoRepository<UserDocument> _userCollection;

        public AddUserHandler(ITokenGeneratorService tokenGeneratorService, IUserValidationService userValidationService, IMapper mapper, IMongoCollectionFactory collectionFactory)
        {
            _tokenGeneratorService = tokenGeneratorService;
            _userValidationService = userValidationService;
            _mapper = mapper;
            _userCollection = collectionFactory.GetCollection<UserDocument>();
        }

        public async Task<UserTokenDto> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            if (!await _userValidationService.ValidUserAsync(request.User))
            {
                throw new InvalidCredentialsException();
            }

            request.User.Password = _userValidationService.HashPassword(request.User.Password);
            UserDocument insertedUser = _userCollection.Insert(_mapper.Map<UserDocument>(request.User));

            return _tokenGeneratorService.GenerateToken(insertedUser);
        }
    }
}
