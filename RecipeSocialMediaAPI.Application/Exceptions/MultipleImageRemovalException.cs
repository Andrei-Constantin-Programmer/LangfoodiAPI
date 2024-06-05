using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class MultipleImageRemovalException : Exception
{
    public MultipleImageRemovalException(List<string> publicIds)
        : base($"Could not remove images or only partially removed some images from: [{string.Join(",", publicIds)}]") { }

    protected MultipleImageRemovalException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
