namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class GroupUpdateException : Exception
{
    public GroupUpdateException(string message) : base(message) { }
}
