using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeNotFoundException : Exception
{
    public RecipeNotFoundException(string recipeId) : base($"The recipe with the id {recipeId} was not found") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected RecipeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
