using AutoMapper;
using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Contracts;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Validation;
using RecipeSocialMediaAPI.Validation.GenericValidators.Interfaces;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

public record UpdateUserCommand(UpdateUserContract UpdateUserContract) : IValidatableRequestVoid;

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
        request.UpdateUserContract.Password = _userValidationService.HashPassword(request.UpdateUserContract.Password);
        UserDocument updatedUserDocument = new()
        {
            Id = request.UpdateUserContract.Id,
            UserName = request.UpdateUserContract.UserName,
            Email = request.UpdateUserContract.Email,
            Password = request.UpdateUserContract.Password,
        };

        var result = _userCollection.UpdateRecord(updatedUserDocument, x => x.Id == updatedUserDocument.Id);

        return result 
            ? Task.CompletedTask 
            : throw new UserNotFoundException();
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public UpdateUserCommandValidator(IUserValidationService userValidationService)
    {
        _userValidationService = userValidationService;

        RuleFor(x => x.UpdateUserContract.UserName)
            .NotEmpty()
            .Must(_userValidationService.ValidUserName);

        RuleFor(x => x.UpdateUserContract.Email)
            .NotEmpty()
            .Must(_userValidationService.ValidEmail);

        RuleFor(x => x.UpdateUserContract.Password)
            .NotEmpty()
            .Must(_userValidationService.ValidPassword);
    }
}
