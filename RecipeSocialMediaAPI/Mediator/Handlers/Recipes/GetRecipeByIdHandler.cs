using MediatR;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Mediator.Queries.Recipes;

namespace RecipeSocialMediaAPI.Mediator.Handlers.Recipes
{
    public class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDTO>
    {
        private readonly IRecipeRepository _repository;

        public GetRecipeByIdHandler(IRecipeRepository repository)
        {
            _repository = repository;
        }

        public async Task<RecipeDTO> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
        {
            var recipe = await _repository.GetRecipeById(request.Id) ?? throw new RecipeNotFoundException(request.Id);

            return new RecipeDTO() 
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Chef = recipe.Chef,
                CreationDate = recipe.CreationDate
            };
        }
    }
}
