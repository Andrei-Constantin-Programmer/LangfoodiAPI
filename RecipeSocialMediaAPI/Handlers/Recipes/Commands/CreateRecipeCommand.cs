using MediatR;
using RecipeSocialMediaAPI.Model;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Utilities;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

namespace RecipeSocialMediaAPI.Handlers.Recipes.Commands;

internal record CreateRecipeCommand(RecipeDTO Recipe) : IRequest;

internal class CreateRecipeHandler : IRequestHandler<CreateRecipeCommand>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateRecipeHandler(IRecipeRepository recipeRepository, IDateTimeProvider dateTimeProvider)
    {
        _recipeRepository = recipeRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipeDTO = request.Recipe;
        await _recipeRepository.CreateRecipe(new Recipe(
            recipeDTO.Id,
            recipeDTO.Title,
            recipeDTO.Description,
            recipeDTO.Chef,
            recipeDTO.CreationDate ?? _dateTimeProvider.Now
        ));
    }
}
