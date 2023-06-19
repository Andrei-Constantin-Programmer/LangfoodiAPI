using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

internal record AddUserCommand(NewUserDTO User) : IRequest<UserDTO>;

internal class AddUserHandler : IRequestHandler<AddUserCommand, UserDTO>
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

    public Task<UserDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        if (!_userValidationService.ValidUser(request.User))
        {
            throw new InvalidCredentialsException();
        }

        if(_userService.DoesUsernameExist(request.User.UserName)
            || _userService.DoesEmailExist(request.User.Email))
        {
            throw new UserAlreadyExistsException();
        }

        request.User.Password = _userValidationService.HashPassword(request.User.Password);
        UserDocument insertedUser = _userCollection.Insert(_mapper.Map<UserDocument>(request.User));

        return Task.FromResult(_mapper.Map<UserDTO>(insertedUser));
    }
}
