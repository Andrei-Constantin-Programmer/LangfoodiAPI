namespace RecipeSocialMediaAPI.Core.Exceptions;

[Serializable]
public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base()
    {
    }
}