using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IRecipeDocumentToModelMapper
{
    Recipe MapRecipeDocumentToRecipe(RecipeDocument recipeDocument, IUserAccount chef);
}
