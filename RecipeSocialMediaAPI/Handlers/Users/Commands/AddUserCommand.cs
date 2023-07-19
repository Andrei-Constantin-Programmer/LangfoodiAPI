using AutoMapper;
using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Contracts;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Validation;
using RecipeSocialMediaAPI.Validation.GenericValidators.Interfaces;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

public record AddUserCommand(NewUserContract NewUserCommand) : IValidatableRequest<UserDTO>;

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
        if(_userService.DoesUsernameExist(request.NewUserCommand.UserName))
        {
            throw new UsernameAlreadyInUseException(request.NewUserCommand.UserName);
        }

        if (_userService.DoesEmailExist(request.NewUserCommand.Email))
        {
            throw new EmailAlreadyInUseException(request.NewUserCommand.Email);
        }

        request.NewUserCommand.Password = _userValidationService.HashPassword(request.NewUserCommand.Password);
        UserDocument insertedUser = _userCollection.Insert(_mapper.Map<UserDocument>(request.NewUserCommand));

        return await Task.FromResult(_mapper.Map<UserDTO>(insertedUser));
    }
}

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public AddUserCommandValidator(IUserValidationService userValidationService)
    {
        _userValidationService = userValidationService;

        RuleFor(x => x.NewUserCommand.UserName)
            .NotEmpty()
            .Must(_userValidationService.ValidUserName);

        RuleFor(x => x.NewUserCommand.Email)
            .NotEmpty()
            .Must(_userValidationService.ValidEmail);

        RuleFor(x => x.NewUserCommand.Password)
            .NotEmpty()
            .Must(_userValidationService.ValidPassword);
    }
}
