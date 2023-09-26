using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Recipes;

public class RecipeQueryRepository : IRecipeQueryRepository
{
    private readonly IRecipeDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<RecipeDocument> _recipeCollection;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly ILogger<RecipeQueryRepository> _logger;

    public RecipeQueryRepository(IRecipeDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory, IUserQueryRepository userQueryRepository, ILogger<RecipeQueryRepository> logger)
    {
        _mapper = mapper;
        _recipeCollection = mongoCollectionFactory.CreateCollection<RecipeDocument>();
        _userQueryRepository = userQueryRepository;
        _logger = logger;
    }

    public RecipeAggregate? GetRecipeById(string id)
    {
        RecipeDocument? recipeDocument = _recipeCollection
            .Find(recipeDoc => recipeDoc.Id == id);

        if (recipeDocument is null)
        {
            return null;
        }

        User? chef = _userQueryRepository.GetUserById(recipeDocument.ChefId);

        if (chef is null)
        {
            _logger.LogWarning("The chef with id {ChefId} was not found for recipe with id {RecipeId}", recipeDocument.ChefId, recipeDocument.Id);
            return null;
        }

        return _mapper.MapRecipeDocumentToRecipeAggregate(recipeDocument, chef);
    }

    public IEnumerable<RecipeAggregate> GetRecipesByChef(User? chef)
    {
        if (chef is null)
        {
            return Enumerable.Empty<RecipeAggregate>();
        }

        var recipes = _recipeCollection
            .GetAll(recipeDoc => recipeDoc.ChefId == chef.Id);

        return recipes.Count == 0
            ? Enumerable.Empty<RecipeAggregate>()
            : recipes.Select(recipeDoc => _mapper.MapRecipeDocumentToRecipeAggregate(recipeDoc, chef));
    }

    public IEnumerable<RecipeAggregate> GetRecipesByChefId(string chefId)
    {
        User? chef = _userQueryRepository.GetUserById(chefId);
        return GetRecipesByChef(chef);
    }

    public IEnumerable<RecipeAggregate> GetRecipesByChefName(string chefName)
    {
        User? chef = _userQueryRepository.GetUserByUsername(chefName);
        return GetRecipesByChef(chef);
    }
}
