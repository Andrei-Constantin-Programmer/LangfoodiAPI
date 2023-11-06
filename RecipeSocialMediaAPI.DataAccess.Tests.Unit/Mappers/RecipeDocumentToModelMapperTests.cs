using RecipeSocialMediaAPI.DataAccess.Mappers;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Mappers;

public class RecipeDocumentToModelMapperTests
{
    private readonly RecipeDocumentToModelMapper _recipeDocumentToModelMapperSUT;

    public RecipeDocumentToModelMapperTests()
    {
        _recipeDocumentToModelMapperSUT = new RecipeDocumentToModelMapper();
    }
}
