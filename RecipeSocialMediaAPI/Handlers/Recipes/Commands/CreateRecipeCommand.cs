using MediatR;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services.Interfaces;

namespace RecipeSocialMediaAPI.Handlers.Recipes.Commands
{
    internal record CreateRecipeCommand(RecipeDTO Recipe) : IRequest;

    internal class CreateRecipeHandler : IRequestHandler<CreateRecipeCommand>
    {
        private readonly IRecipeRepository _fakeRecipeRepository;
        private readonly IClock _dateTimeProvider;

        public CreateRecipeHandler(IRecipeRepository fakeRecipeRepository, IClock dateTimeProvider)
        {
            _fakeRecipeRepository = fakeRecipeRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
        {
            var recipeDTO = request.Recipe;
            await _fakeRecipeRepository.CreateRecipe(new Recipe(
                recipeDTO.Id,
                recipeDTO.Title,
                recipeDTO.Description,
                recipeDTO.Chef,
                recipeDTO.CreationDate ?? _dateTimeProvider.Now
            ));
        }
    }
}
