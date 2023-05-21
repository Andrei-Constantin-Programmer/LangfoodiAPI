using MediatR;
using RecipeSocialMediaAPI.DTO;

namespace RecipeSocialMediaAPI.Mediator.Commands.Recipes
{
    public record CreateRecipeCommand(RecipeDTO Recipe) : IRequest;
}
