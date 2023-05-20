using MediatR;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data;
using RecipeSocialMediaAPI.Mediator.Commands.Recipes;
using RecipeSocialMediaAPI.Services.Interfaces;

namespace RecipeSocialMediaAPI.Mediator.Handlers.Recipes
{
    public class AddRecipeHandler : IRequestHandler<AddRecipeCommand>
    {
        private readonly IFakeRecipeRepository _fakeRecipeRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        
        public AddRecipeHandler(IFakeRecipeRepository fakeRecipeRepository, IDateTimeProvider dateTimeProvider)
        {
            _fakeRecipeRepository = fakeRecipeRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task Handle(AddRecipeCommand request, CancellationToken cancellationToken)
        {
            await _fakeRecipeRepository.AddRecipe(new Recipe(
                request.Recipe.Title,
                request.Recipe.Description,
                request.Recipe.Chef,
                request.Recipe.CreationDate ?? _dateTimeProvider.Now
            ));
        }
    }
}
