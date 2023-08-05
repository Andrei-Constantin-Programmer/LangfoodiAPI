using MediatR;
using RecipeSocialMediaAPI.Domain.Entities;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

internal record CreateRecipeCommand(RecipeDTO Recipe) : IRequest;

internal class CreateRecipeHandler : IRequestHandler<CreateRecipeCommand>
{
    public CreateRecipeHandler(IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
    }

    public async Task Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
