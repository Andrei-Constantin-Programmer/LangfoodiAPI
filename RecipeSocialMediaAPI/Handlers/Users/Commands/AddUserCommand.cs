using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    public record AddUserCommand(UserDto User) : IRequest<UserTokenDto>;

    public class AddUserHandler : IRequestHandler<AddUserCommand, UserTokenDto>
    {
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly IUserValidationService _userValidationService;
        private readonly IMapper _mapper;

        private readonly IMongoCollectionWrapper<UserDocument> _userCollection;

        public AddUserHandler(ITokenGeneratorService tokenGeneratorService, IUserValidationService userValidationService, IMapper mapper, IMongoFactory factory, IConfigManager config)
        {
            _tokenGeneratorService = tokenGeneratorService;
            _userValidationService = userValidationService;
            _mapper = mapper;
            _userCollection = factory.GetCollection<UserDocument>(new UserRepository(), config);
        }

        public async Task<UserTokenDto> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            if (!await _userValidationService.ValidUserAsync(request.User))
            {
                throw new InvalidCredentialsException();
            }

            var user = request.User;
            user.Password = _userValidationService.HashPassword(user.Password);
            UserDocument insertedUser = _userCollection.Insert(_mapper.Map<UserDocument>(user));

            return _tokenGeneratorService.GenerateToken(insertedUser);
        }
    }
}
