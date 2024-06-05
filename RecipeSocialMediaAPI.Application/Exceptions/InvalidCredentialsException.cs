using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base()
    {
    }

    protected InvalidCredentialsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}