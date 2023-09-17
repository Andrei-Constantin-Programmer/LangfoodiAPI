using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Core.Contracts.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Mappers.Recipes;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

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
        if (existingRecipe == null)
        {
            throw new RecipeNotFoundException(request.UpdateRecipeContract.Id);
        }

        RecipeAggregate updatedRecipe = new(
            existingRecipe.Id,
            request.UpdateRecipeContract.Title,
            new Recipe(
                request.UpdateRecipeContract.Ingredients
                    .Select(_mapper.IngredientMapper.MapIngredientDtoToIngredient)
                    .ToList(),
                new Stack<RecipeStep>(request.UpdateRecipeContract.RecipeSteps
                    .Select(_mapper.RecipeStepMapper.MapRecipeStepDtoToRecipeStep))
            ),
            request.UpdateRecipeContract.Description,
            existingRecipe.Chef,
            existingRecipe.CreationDate,
            _dateTimeProvider.Now,
            request.UpdateRecipeContract.Labels,
            request.UpdateRecipeContract.NumberOfServings ?? existingRecipe.NumberOfServings,
            request.UpdateRecipeContract.CookingTime ?? existingRecipe.CookingTimeInSeconds,
            request.UpdateRecipeContract.KiloCalories ?? existingRecipe.KiloCalories
        );

        bool isSuccessful = _recipeRepository.UpdateRecipe(updatedRecipe);

        return isSuccessful
            ? Task.CompletedTask
            : throw new Exception($"Could not update recipe with id {existingRecipe.Id}.");
    }
}

public class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    public UpdateRecipeCommandValidator()
    {
        RuleFor(x => x.UpdateRecipeContract.Ingredients)
            .NotEmpty();

        RuleFor(x => x.UpdateRecipeContract.RecipeSteps)
            .NotEmpty();
    }
}
