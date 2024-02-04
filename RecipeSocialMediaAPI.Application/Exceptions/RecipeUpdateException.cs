namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeUpdateException : Exception
{
    public RecipeUpdateException(string message) : base(message) { }
}
