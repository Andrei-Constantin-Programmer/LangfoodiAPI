using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeNotFoundException : Exception
{
    public RecipeNotFoundException(string recipeId) : base($"The recipe with the id {recipeId} was not found") { }

    protected RecipeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
