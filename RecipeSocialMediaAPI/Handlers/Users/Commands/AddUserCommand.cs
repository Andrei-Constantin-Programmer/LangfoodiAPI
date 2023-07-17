using AutoMapper;
using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Validation;
using RecipeSocialMediaAPI.Validation.GenericValidators.Interfaces;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

public record AddUserCommand(NewUserDTO User) : IValidatableRequest<UserDTO>;

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

    public async Task<UserDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
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

        RuleFor(x => x.User.UserName)
            .NotEmpty()
            .Must(_userValidationService.ValidUserName);

        RuleFor(x => x.User.Email)
            .NotEmpty()
            .Must(_userValidationService.ValidEmail);

        RuleFor(x => x.User.Password)
            .NotEmpty()
            .Must(_userValidationService.ValidPassword);
    }
}
