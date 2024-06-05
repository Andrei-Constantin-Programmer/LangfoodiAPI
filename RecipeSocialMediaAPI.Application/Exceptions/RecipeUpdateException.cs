using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeUpdateException : Exception
{
    public RecipeUpdateException(string message) : base(message) { }

    protected RecipeUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
