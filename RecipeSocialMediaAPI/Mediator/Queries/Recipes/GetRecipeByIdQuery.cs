using MediatR;
using RecipeSocialMediaAPI.DTO;

namespace RecipeSocialMediaAPI.Mediator.Queries.Recipes
{
    public record GetRecipeByIdQuery(int Id) : IRequest<RecipeDTO>;
}
