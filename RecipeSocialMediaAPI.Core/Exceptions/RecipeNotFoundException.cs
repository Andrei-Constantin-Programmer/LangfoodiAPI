namespace RecipeSocialMediaAPI.Core.Exceptions;

public class RecipeNotFoundException : Exception
{
    public RecipeNotFoundException(string id) : base($"The recipe with the id {id} was not found.") { }
}
