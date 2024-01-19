namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeNotFoundException : Exception
{
    public RecipeNotFoundException(string id) : base($"The recipe with the id {id} was not found") { }
}
