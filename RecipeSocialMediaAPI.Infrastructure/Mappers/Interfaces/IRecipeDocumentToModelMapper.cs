using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IRecipeDocumentToModelMapper
{
    public RecipeAggregate MapRecipeDocumentToRecipeAggregate(RecipeDocument recipeDocument, IUserAccount chef);
}
