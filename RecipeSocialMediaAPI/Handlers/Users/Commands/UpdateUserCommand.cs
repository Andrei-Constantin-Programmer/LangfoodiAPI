using AutoMapper;
using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Validation;
using RecipeSocialMediaAPI.Validation.GenericValidators.Interfaces;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

public record UpdateUserCommand(UserDTO User) : IValidatableRequestVoid;

internal class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserValidationService _userValidationService;
    private readonly IMapper _mapper;

    private readonly IMongoRepository<UserDocument> _userCollection;

    public UpdateUserHandler(IUserValidationService userValidationService, IMapper mapper, IMongoCollectionFactory collectionFactory)
    {
        _userValidationService = userValidationService;
        _mapper = mapper;

        _userCollection = collectionFactory.GetCollection<UserDocument>();
    }

    public Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        request.User.Password = _userValidationService.HashPassword(request.User.Password);
        UserDocument newUserDoc = _mapper.Map<UserDocument>(request.User);

        var result = _userCollection.UpdateRecord(newUserDoc, x => x.Id == newUserDoc.Id);

        return result 
            ? Task.CompletedTask 
            : throw new UserNotFoundException();
    }
}

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public UpdateUserValidator(IUserValidationService userValidationService)
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
