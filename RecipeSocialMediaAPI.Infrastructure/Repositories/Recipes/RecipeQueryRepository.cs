using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Recipes;

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

    public async Task<RecipeAggregate?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        RecipeDocument? recipeDocument;
        try
        {
            recipeDocument = await _recipeCollection
                .GetOneAsync(recipeDoc => recipeDoc.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error trying to get recipe by id {RecipeId}: {ErrorMessage}", id, ex.Message);
            recipeDocument = null;
        }

        if (recipeDocument is null)
        {
            return null;
        }

        IUserAccount? chef = (await _userQueryRepository.GetUserByIdAsync(recipeDocument.ChefId, cancellationToken))?.Account;

        if (chef is null)
        {
            _logger.LogWarning("The chef with id {ChefId} was not found for recipe with id {RecipeId}", recipeDocument.ChefId, recipeDocument.Id);
            return null;
        }

        return _mapper.MapRecipeDocumentToRecipeAggregate(recipeDocument, chef);
    }

    public async Task<IEnumerable<RecipeAggregate>> GetRecipesByChefAsync(IUserAccount? chef, CancellationToken cancellationToken = default)
    {
        if (chef is null)
        {
            return Enumerable.Empty<RecipeAggregate>();
        }

        IEnumerable<RecipeDocument> recipes = Enumerable.Empty<RecipeDocument>();
        try
        {
            recipes = await _recipeCollection
                .GetAllAsync(recipeDoc => recipeDoc.ChefId == chef.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "There was an error trying to get recipes for chef with id {ChefId}: {ErrorMessage}", chef.Id, ex.Message);
        }

        return recipes.Select(recipeDoc => _mapper.MapRecipeDocumentToRecipeAggregate(recipeDoc, chef));
    }

    public async Task<IEnumerable<RecipeAggregate>> GetRecipesByChefIdAsync(string chefId, CancellationToken cancellationToken = default)
    {
        IUserAccount? chef = (await _userQueryRepository.GetUserByIdAsync(chefId, cancellationToken))?.Account;
        return await GetRecipesByChefAsync(chef, cancellationToken);
    }

    public async Task<IEnumerable<RecipeAggregate>> GetRecipesByChefNameAsync(string chefName, CancellationToken cancellationToken = default)
    {
        IUserAccount? chef = (await _userQueryRepository.GetUserByUsernameAsync(chefName, cancellationToken))?.Account;
        return await GetRecipesByChefAsync(chef, cancellationToken);
    }
}
