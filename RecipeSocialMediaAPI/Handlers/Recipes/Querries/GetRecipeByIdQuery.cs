using MediatR;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Exceptions;

namespace RecipeSocialMediaAPI.Handlers.Recipes.Queries
{
    internal record GetRecipeByIdQuery(int Id) : IRequest<RecipeDTO>;

    internal class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDTO>
    {
        private readonly IRecipeRepository _recipeRepository;

        public GetRecipeByIdHandler(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<RecipeDTO> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
        {
            var recipe = await _recipeRepository.GetRecipeById(request.Id) ?? throw new RecipeNotFoundException(request.Id);

            return new RecipeDTO()
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Chef = recipe.Chef,
                CreationDate = recipe.CreationDate,
            };
        }
    }
}
