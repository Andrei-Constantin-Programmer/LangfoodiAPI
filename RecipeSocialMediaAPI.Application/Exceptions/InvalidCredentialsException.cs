namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base()
    {
    }
}