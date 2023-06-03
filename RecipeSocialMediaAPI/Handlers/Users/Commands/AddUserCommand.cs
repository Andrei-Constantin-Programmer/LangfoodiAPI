using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

internal record AddUserCommand(UserDto User) : IRequest<UserDto>;

internal class AddUserHandler : IRequestHandler<AddUserCommand, UserDto>
{
    private readonly IUserValidationService _userValidationService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    private readonly IMongoRepository<UserDocument> _userCollection;

    public AddUserHandler(IUserValidationService userValidationService, IUserService userService, IMapper mapper, IMongoCollectionFactory collectionFactory)
    {
        _userValidationService = userValidationService;
        _userService = userService;
        _mapper = mapper;
        _userCollection = collectionFactory.GetCollection<UserDocument>();
    }

    public Task<UserDto> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        if (!_userValidationService.ValidUser(request.User))
        {
            throw new InvalidCredentialsException();
        }

        if (request.User.Id is not null)
        {
            throw new ArgumentException("You cannot insert a user with a predefined Id.");
        }

        if(_userService.DoesUsernameExist(request.User.UserName)
            || _userService.DoesEmailExist(request.User.Email))
        {
            throw new UserAlreadyExistsException();
        }

        request.User.Password = _userValidationService.HashPassword(request.User.Password);
        UserDocument insertedUser = _userCollection.Insert(_mapper.Map<UserDocument>(request.User));

        return Task.FromResult(_mapper.Map<UserDto>(insertedUser));
    }
}
