using MediatR;
using RecipeSocialMediaAPI.DTO;

namespace RecipeSocialMediaAPI.Mediator.Queries.Recipes
{
    public record GetRecipesQuery() : IRequest<IEnumerable<RecipeDTO>>;
}
