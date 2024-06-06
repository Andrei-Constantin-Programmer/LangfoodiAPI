using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeRemovalException : Exception
{
    public RecipeRemovalException(string recipeId) : base($"Could not remove recipe with id {recipeId}") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected RecipeRemovalException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
