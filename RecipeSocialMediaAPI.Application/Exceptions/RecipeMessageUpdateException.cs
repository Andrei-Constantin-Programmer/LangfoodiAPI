namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeMessageUpdateException : MessageUpdateException
{
    public RecipeMessageUpdateException(string messageId, string reason) : base($"Cannot update recipe message with id {messageId}: {reason}") { }
}
