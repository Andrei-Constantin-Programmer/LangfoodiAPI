using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeUpdateException : Exception
{
    public RecipeUpdateException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected RecipeUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
