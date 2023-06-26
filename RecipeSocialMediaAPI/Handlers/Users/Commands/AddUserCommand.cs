using AutoMapper;
using FluentValidation;
using MediatR;
using OneOf;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Validation;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

public record AddUserCommand(NewUserDTO User) : IRequest<OneOf<UserDTO, ValidationFailed>>;

internal class AddUserHandler : IRequestHandler<AddUserCommand, OneOf<UserDTO, ValidationFailed>>
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

    public async Task<OneOf<UserDTO, ValidationFailed>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        if(_userService.DoesUsernameExist(request.User.UserName)
            || _userService.DoesEmailExist(request.User.Email))
        {
            throw new UserAlreadyExistsException();
        }

        request.User.Password = _userValidationService.HashPassword(request.User.Password);
        UserDocument insertedUser = _userCollection.Insert(_mapper.Map<UserDocument>(request.User));

        return await Task.FromResult(_mapper.Map<UserDTO>(insertedUser));
    }
}

public class AddUserValidator : AbstractValidator<AddUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public AddUserValidator(IUserValidationService userValidationService)
    {
        _userValidationService = userValidationService;

        RuleFor(x => x.User)
            .NotEmpty()
            .Must(_userValidationService.ValidUser);
    }
}
