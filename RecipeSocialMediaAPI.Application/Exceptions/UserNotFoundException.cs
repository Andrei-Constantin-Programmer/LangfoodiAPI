namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base()
    {
    }
}