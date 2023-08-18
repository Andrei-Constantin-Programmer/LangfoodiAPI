using MediatR;
using RecipeSocialMediaAPI.Domain.Models;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.Core.DTO.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record AddRecipeCommand(NewRecipeContract NewRecipeContract) : IValidatableRequest<RecipeDetailedDTO>;

internal class AddRecipeHandler : IRequestHandler<AddRecipeCommand, RecipeDetailedDTO>
{
    public AddRecipeHandler(IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
    }

    public async Task<RecipeDetailedDTO> Handle(AddRecipeCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
