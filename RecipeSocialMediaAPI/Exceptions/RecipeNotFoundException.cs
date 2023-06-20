namespace RecipeSocialMediaAPI.Exceptions
{
    public class RecipeNotFoundException : Exception
    {
        public RecipeNotFoundException(int id) : base($"The recipe with the id {id} was not found.") { }
    }
}
