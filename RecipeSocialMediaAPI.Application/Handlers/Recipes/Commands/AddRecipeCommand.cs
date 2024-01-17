using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;

public record AddRecipeCommand(NewRecipeContract Contract) : IValidatableRequest<RecipeDetailedDTO>;

internal class AddRecipeHandler : IRequestHandler<AddRecipeCommand, RecipeDetailedDTO>
{
    private readonly IRecipeMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRecipePersistenceRepository _recipePersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public AddRecipeHandler(IRecipeMapper mapper, IUserQueryRepository userQueryRepository, IRecipePersistenceRepository recipePersistenceRepository, IDateTimeProvider dateTimeProvider)
    {
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _recipePersistenceRepository = recipePersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<RecipeDetailedDTO> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        IUserAccount? chef = 
            _userQueryRepository.GetUserById(request.Contract.ChefId)?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.Contract.ChefId}");

        DateTimeOffset dateOfCreation = _dateTimeProvider.Now;

        RecipeAggregate insertedRecipe = _recipePersistenceRepository.CreateRecipe(
            request.Contract.Title,
            new Recipe(
                request.Contract.Ingredients
                    .Select(_mapper.MapIngredientDtoToIngredient)
                    .ToList(),
                new Stack<RecipeStep>(request.Contract.RecipeSteps
                    .Select(_mapper.MapRecipeStepDtoToRecipeStep)),
                request.Contract.NumberOfServings,
                request.Contract.CookingTime,
                request.Contract.KiloCalories,
                request.Contract.ServingSize is not null 
                    ? _mapper.MapServingSizeDtoToServingSize(request.Contract.ServingSize)
                    : null
            ),
            request.Contract.Description,
            chef,
            request.Contract.Tags,
            dateOfCreation,
            dateOfCreation,
            request.Contract.ThumbnailId
        );

        return await Task.FromResult(_mapper
            .MapRecipeAggregateToRecipeDetailedDto(insertedRecipe));
    }
}

public class AddRecipeCommandValidator : AbstractValidator<AddRecipeCommand>
{
    private readonly IRecipeValidationService _recipeValidationService;

    public AddRecipeCommandValidator(IRecipeValidationService recipeValidationService)
    {
        _recipeValidationService = recipeValidationService;

        RuleFor(x => x.Contract.Title)
            .Must(_recipeValidationService.ValidTitle);

        RuleFor(x => x.Contract.NumberOfServings)
            .GreaterThanOrEqualTo(1)
            .When(x => x.Contract.NumberOfServings is not null);

        RuleFor(x => x.Contract.CookingTime)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Contract.CookingTime is not null);

        RuleFor(x => x.Contract.KiloCalories)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Contract.KiloCalories is not null);

        RuleFor(x => x.Contract.Ingredients)
            .NotEmpty();

        RuleFor(x => x.Contract.RecipeSteps)
            .NotEmpty();
    }
}