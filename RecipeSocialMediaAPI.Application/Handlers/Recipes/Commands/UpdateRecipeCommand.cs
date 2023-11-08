using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;

public record UpdateRecipeCommand(UpdateRecipeContract UpdateRecipeContract) : IValidatableRequest;

internal class UpdateRecipeHandler : IRequestHandler<UpdateRecipeCommand>
{
    private readonly IRecipeMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRecipePersistenceRepository _recipePersistenceRepository;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    public UpdateRecipeHandler(IRecipeMapper mapper, IRecipePersistenceRepository recipePersistenceRepository, IRecipeQueryRepository recipeQueryRepository, IDateTimeProvider dateTimeProvider)
    {
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _recipePersistenceRepository = recipePersistenceRepository;
        _recipeQueryRepository = recipeQueryRepository;
    }

    public Task Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        RecipeAggregate existingRecipe = 
            _recipeQueryRepository.GetRecipeById(request.UpdateRecipeContract.Id) 
            ?? throw new RecipeNotFoundException(request.UpdateRecipeContract.Id);

        var (servingSizeQuantity, unitOfMeasurement) = request.UpdateRecipeContract.ServingSize ?? default;

        RecipeAggregate updatedRecipe = new(
            existingRecipe.Id,
            request.UpdateRecipeContract.Title,
            new Recipe(
                request.UpdateRecipeContract.Ingredients
                    .Select(_mapper.MapIngredientDtoToIngredient)
                    .ToList(),
                new Stack<RecipeStep>(request.UpdateRecipeContract.RecipeSteps
                    .Select(_mapper.MapRecipeStepDtoToRecipeStep)),
                request.UpdateRecipeContract.NumberOfServings ?? existingRecipe.Recipe.NumberOfServings,
                request.UpdateRecipeContract.CookingTime ?? existingRecipe.Recipe.CookingTimeInSeconds,
                request.UpdateRecipeContract.KiloCalories ?? existingRecipe.Recipe.KiloCalories,
                request.UpdateRecipeContract.ServingSize is not null 
                    ? new ServingSize(servingSizeQuantity, unitOfMeasurement) 
                    : null
            ),
            request.UpdateRecipeContract.Description,
            existingRecipe.Chef,
            existingRecipe.CreationDate,
            _dateTimeProvider.Now,
            request.UpdateRecipeContract.Labels
        );

        bool isSuccessful = _recipePersistenceRepository.UpdateRecipe(updatedRecipe);

        return isSuccessful
            ? Task.CompletedTask
            : throw new Exception($"Could not update recipe with id {existingRecipe.Id}");
    }
}

public class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    private readonly IRecipeValidationService _recipeValidationService;
    public UpdateRecipeCommandValidator(IRecipeValidationService recipeValidationService)
    {
        _recipeValidationService = recipeValidationService;

        RuleFor(x => x.UpdateRecipeContract.Title)
            .Must(_recipeValidationService.ValidTitle);

        RuleFor(x => x.UpdateRecipeContract.NumberOfServings)
            .GreaterThanOrEqualTo(1)
            .When(x => x.UpdateRecipeContract.NumberOfServings is not null);

        RuleFor(x => x.UpdateRecipeContract.CookingTime)
            .GreaterThanOrEqualTo(0)
            .When(x => x.UpdateRecipeContract.CookingTime is not null);

        RuleFor(x => x.UpdateRecipeContract.KiloCalories)
            .GreaterThanOrEqualTo(0)
            .When(x => x.UpdateRecipeContract.KiloCalories is not null);

        RuleFor(x => x.UpdateRecipeContract.Ingredients)
            .NotEmpty();

        RuleFor(x => x.UpdateRecipeContract.RecipeSteps)
            .NotEmpty();
    }
}
