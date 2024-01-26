namespace RecipeSocialMediaAPI.Application.Exceptions;
public class MultipleImageRemovalException : Exception
{
    public MultipleImageRemovalException(List<string> publicIds) 
        : base($"Could not remove images or only partially removed some images from: [{string.Join(",", publicIds)}]") { }
}
