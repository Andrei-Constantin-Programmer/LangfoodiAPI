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

public record AddRecipeCommand(NewRecipeContract NewRecipeContract) : IValidatableRequest<RecipeDetailedDTO>;

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
            _userQueryRepository.GetUserById(request.NewRecipeContract.ChefId)?.Account
            ?? throw new UserNotFoundException();

        DateTimeOffset dateOfCreation = _dateTimeProvider.Now;
        RecipeAggregate insertedRecipe = _recipePersistenceRepository.CreateRecipe(
            request.NewRecipeContract.Title,
            new Recipe(
                request.NewRecipeContract.Ingredients
                    .Select(_mapper.MapIngredientDtoToIngredient)
                    .ToList(),
                new Stack<RecipeStep>(request.NewRecipeContract.RecipeSteps
                    .Select(_mapper.MapRecipeStepDtoToRecipeStep)),
                request.NewRecipeContract.NumberOfServings,
                request.NewRecipeContract.CookingTime,
                request.NewRecipeContract.KiloCalories
            ),
            request.NewRecipeContract.Description,
            chef,
            request.NewRecipeContract.Labels,
            dateOfCreation,
            dateOfCreation
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

        RuleFor(x => x.NewRecipeContract.Title)
            .Must(_recipeValidationService.ValidTitle);

        RuleFor(x => x.NewRecipeContract.NumberOfServings)
            .GreaterThanOrEqualTo(1)
            .When(x => x.NewRecipeContract.NumberOfServings is not null);

        RuleFor(x => x.NewRecipeContract.CookingTime)
            .GreaterThanOrEqualTo(0)
            .When(x => x.NewRecipeContract.CookingTime is not null);

        RuleFor(x => x.NewRecipeContract.KiloCalories)
            .GreaterThanOrEqualTo(0)
            .When(x => x.NewRecipeContract.KiloCalories is not null);

        RuleFor(x => x.NewRecipeContract.Ingredients)
            .NotEmpty();

        RuleFor(x => x.NewRecipeContract.RecipeSteps)
            .NotEmpty();
    }
}