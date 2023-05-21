using MediatR;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data;
using RecipeSocialMediaAPI.Mediator.Commands.Recipes;
using RecipeSocialMediaAPI.Services.Interfaces;

namespace RecipeSocialMediaAPI.Mediator.Handlers.Recipes
{
    public class CreateRecipeHandler : IRequestHandler<CreateRecipeCommand>
    {
        private readonly IRecipeRepository _fakeRecipeRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        
        public CreateRecipeHandler(IRecipeRepository fakeRecipeRepository, IDateTimeProvider dateTimeProvider)
        {
            _fakeRecipeRepository = fakeRecipeRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
        {
            await _fakeRecipeRepository.CreateRecipe(new Recipe(
                request.Recipe.Title,
                request.Recipe.Description,
                request.Recipe.Chef,
                request.Recipe.CreationDate ?? _dateTimeProvider.Now
            ));
        }
    }
}
