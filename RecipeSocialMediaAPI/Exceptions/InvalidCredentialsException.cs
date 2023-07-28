namespace RecipeSocialMediaAPI.Core.Exceptions;

[Serializable]
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base()
    {
    }
}