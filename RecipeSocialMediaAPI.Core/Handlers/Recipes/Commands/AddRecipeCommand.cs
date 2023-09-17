using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Core.Contracts.Recipes;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Mappers.Recipes;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record AddRecipeCommand(NewRecipeContract NewRecipeContract) : IValidatableRequest<RecipeDetailedDTO>;

internal class AddRecipeHandler : IRequestHandler<AddRecipeCommand, RecipeDetailedDTO>
{
    private readonly IRecipeMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;

    public AddRecipeHandler(IRecipeMapper mapper, IUserRepository userRepository, IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
    }

    public async Task<RecipeDetailedDTO> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        User? chef = _userRepository.GetUserById(request.NewRecipeContract.ChefId);
        if (chef is null)
        {
            throw new UserNotFoundException();
        }

        DateTimeOffset dateOfCreation = _dateTimeProvider.Now;
        RecipeAggregate insertedRecipe = _recipeRepository.CreateRecipe(
            request.NewRecipeContract.Title,
            new Recipe(
                request.NewRecipeContract.Ingredients
                    .Select(_mapper.IngredientMapper.MapIngredientDtoToIngredient)
                    .ToList(),
                new Stack<RecipeStep>(request.NewRecipeContract.RecipeSteps
                    .Select(_mapper.RecipeStepMapper.MapRecipeStepDtoToRecipeStep))
            ),
            request.NewRecipeContract.Description,
            chef,
            request.NewRecipeContract.Labels,
            request.NewRecipeContract.NumberOfServings,
            request.NewRecipeContract.CookingTime,
            request.NewRecipeContract.KiloCalories,
            dateOfCreation,
            dateOfCreation
        );

        return await Task.FromResult(_mapper.RecipeAggregateToRecipeDetailedDtoMapper
            .MapRecipeAggregateToRecipeDetailedDto(insertedRecipe));
    }
}

public class AddRecipeCommandValidator : AbstractValidator<AddRecipeCommand>
{
    public AddRecipeCommandValidator()
    {
        RuleFor(x => x.NewRecipeContract.Ingredients)
            .NotEmpty();

        RuleFor(x => x.NewRecipeContract.RecipeSteps)
            .NotEmpty();
    }
}