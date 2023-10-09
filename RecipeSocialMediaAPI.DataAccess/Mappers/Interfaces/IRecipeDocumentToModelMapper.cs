using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;

public interface IRecipeDocumentToModelMapper
{
    public RecipeAggregate MapRecipeDocumentToRecipeAggregate(RecipeDocument recipeDocument, IUserAccount chef);
}
