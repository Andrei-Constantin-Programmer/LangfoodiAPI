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

public record UpdateRecipeCommand(UpdateRecipeContract Contract) : IValidatableRequest;

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

    public async Task Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        RecipeAggregate existingRecipe = (await _recipeQueryRepository.GetRecipeById(request.Contract.Id, cancellationToken))
            ?? throw new RecipeNotFoundException(request.Contract.Id);

        RecipeAggregate updatedRecipe = new(
            existingRecipe.Id,
            request.Contract.Title,
            new Recipe(
                request.Contract.Ingredients
                    .Select(_mapper.MapIngredientDtoToIngredient)
                    .ToList(),
                new Stack<RecipeStep>(request.Contract.RecipeSteps
                    .Select(_mapper.MapRecipeStepDtoToRecipeStep)),
                request.Contract.NumberOfServings ?? existingRecipe.Recipe.NumberOfServings,
                request.Contract.CookingTime ?? existingRecipe.Recipe.CookingTimeInSeconds,
                request.Contract.KiloCalories ?? existingRecipe.Recipe.KiloCalories,
                request.Contract.ServingSize is not null
                    ? _mapper.MapServingSizeDtoToServingSize(request.Contract.ServingSize)
                    : null
            ),
            request.Contract.Description,
            existingRecipe.Chef,
            existingRecipe.CreationDate,
            _dateTimeProvider.Now,
            request.Contract.Tags,
            request.Contract.ThumbnailId
        );

        bool isSuccessful = await _recipePersistenceRepository.UpdateRecipe(updatedRecipe, cancellationToken);

        if (!isSuccessful)
        {
            throw new RecipeUpdateException($"Could not update recipe with id {existingRecipe.Id}");
        }
    }
}

public class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    private readonly IRecipeValidationService _recipeValidationService;

    public UpdateRecipeCommandValidator(IRecipeValidationService recipeValidationService)
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
