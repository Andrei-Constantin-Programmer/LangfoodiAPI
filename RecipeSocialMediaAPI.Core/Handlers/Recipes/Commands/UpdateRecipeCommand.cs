using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Core.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record UpdateRecipeCommand(UpdateRecipeContract UpdateRecipeContract) : IValidatableRequestVoid;

internal class UpdateRecipeHandler : IRequestHandler<UpdateRecipeCommand>
{
    private readonly IRecipeMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRecipeRepository _recipeRepository;

    public UpdateRecipeHandler(IRecipeMapper mapper, IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _recipeRepository = recipeRepository;
    }

    public Task Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        RecipeAggregate? existingRecipe = _recipeRepository.GetRecipeById(request.UpdateRecipeContract.Id);
        if (existingRecipe is null)
        {
            throw new RecipeNotFoundException(request.UpdateRecipeContract.Id);
        }

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
                request.UpdateRecipeContract.KiloCalories ?? existingRecipe.Recipe.KiloCalories
            ),
            request.UpdateRecipeContract.Description,
            existingRecipe.Chef,
            existingRecipe.CreationDate,
            _dateTimeProvider.Now,
            request.UpdateRecipeContract.Labels
        );

        bool isSuccessful = _recipeRepository.UpdateRecipe(updatedRecipe);

        return isSuccessful
            ? Task.CompletedTask
            : throw new Exception($"Could not update recipe with id {existingRecipe.Id}.");
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
