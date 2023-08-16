using MediatR;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record UpdateRecipeCommand(UpdateRecipeContract UpdateRecipeContract) : IValidatableRequest<RecipeDetailedDTO>;

internal class UpdateRecipeHandler : IRequestHandler<UpdateRecipeCommand, RecipeDetailedDTO>
{
    public UpdateRecipeHandler(IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
    }

    public async Task<RecipeDetailedDTO> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
