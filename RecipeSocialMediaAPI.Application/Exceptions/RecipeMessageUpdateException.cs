using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeMessageUpdateException : MessageUpdateException
{
    public RecipeMessageUpdateException(string messageId, string reason) : base($"Cannot update recipe message with id {messageId}: {reason}") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected RecipeMessageUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
