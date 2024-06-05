using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeNotFoundException : Exception
{
    public RecipeNotFoundException(string id) : base($"The recipe with the id {id} was not found") { }

    protected RecipeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
