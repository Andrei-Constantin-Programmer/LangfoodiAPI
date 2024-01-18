namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class GroupRemovalException : Exception
{
    public GroupRemovalException(string groupId) : base($"Could not remove group with id {groupId}") { }
}
