using MediatR;
using RecipeSocialMediaAPI.DTO;

namespace RecipeSocialMediaAPI.Mediator.Commands.Recipes
{
    public record AddRecipeCommand(RecipeDTO Recipe) : IRequest;
}
