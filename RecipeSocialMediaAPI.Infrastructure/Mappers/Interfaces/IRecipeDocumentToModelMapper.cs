using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IRecipeDocumentToModelMapper
{
    RecipeAggregate MapRecipeDocumentToRecipeAggregate(RecipeDocument recipeDocument, IUserAccount chef);
}
