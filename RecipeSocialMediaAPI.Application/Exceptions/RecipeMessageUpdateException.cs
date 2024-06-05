using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeMessageUpdateException : MessageUpdateException
{
    public RecipeMessageUpdateException(string messageId, string reason) : base($"Cannot update recipe message with id {messageId}: {reason}") { }

    protected RecipeMessageUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
