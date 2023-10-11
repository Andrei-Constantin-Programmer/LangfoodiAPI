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
        RecipeDocument? recipeDocument;
        try
        {
            recipeDocument = _recipeCollection
                .Find(recipeDoc => recipeDoc.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "There was an error trying to get recipe by id {RecipeId}: {ErrorMessage}", id, ex.Message);
            recipeDocument = null;
        }

        if (recipeDocument is null)
        {
            return null;
        }

        IUserAccount? chef = _userQueryRepository.GetUserById(recipeDocument.ChefId)?.Account;

        if (chef is null)
        {
            _logger.LogWarning("The chef with id {ChefId} was not found for recipe with id {RecipeId}", recipeDocument.ChefId, recipeDocument.Id);
            return null;
        }

        return _mapper.MapRecipeDocumentToRecipeAggregate(recipeDocument, chef);
    }

    public IEnumerable<RecipeAggregate> GetRecipesByChef(IUserAccount? chef)
    {
        if (chef is null)
        {
            return Enumerable.Empty<RecipeAggregate>();
        }

        List<RecipeDocument> recipes;
        try
        {
            recipes = _recipeCollection
                .GetAll(recipeDoc => recipeDoc.ChefId == chef.Id);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "There was an error trying to get recipes for chef with id {ChefId}: {ErrorMessage}", chef.Id, ex.Message);
            recipes = new();
        }

        return recipes.Count == 0
            ? Enumerable.Empty<RecipeAggregate>()
            : recipes.Select(recipeDoc => _mapper.MapRecipeDocumentToRecipeAggregate(recipeDoc, chef));
    }

    public IEnumerable<RecipeAggregate> GetRecipesByChefId(string chefId)
    {
        IUserAccount? chef = _userQueryRepository.GetUserById(chefId)?.Account;
        return GetRecipesByChef(chef);
    }

    public IEnumerable<RecipeAggregate> GetRecipesByChefName(string chefName)
    {
        IUserAccount? chef = _userQueryRepository.GetUserByUsername(chefName)?.Account;
        return GetRecipesByChef(chef);
    }
}
