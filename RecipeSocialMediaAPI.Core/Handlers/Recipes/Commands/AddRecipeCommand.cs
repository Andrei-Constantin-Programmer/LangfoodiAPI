using MediatR;
using RecipeSocialMediaAPI.Domain.Models;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using FluentValidation;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record AddRecipeCommand(NewRecipeContract NewRecipeContract) : IValidatableRequest<RecipeDetailedDTO>;

internal class AddRecipeHandler : IRequestHandler<AddRecipeCommand, RecipeDetailedDTO>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;

    public AddRecipeHandler(IUserRepository userRepository, IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
    }

    public async Task<RecipeDetailedDTO> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        if (_userRepository.GetUserById(request.NewRecipeContract.ChefId) is null)
        {
            throw new UserNotFoundException();
        }

        _recipeRepository.CreateRecipe();
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