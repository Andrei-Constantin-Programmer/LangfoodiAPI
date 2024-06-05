using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class HandlerAlreadyInUseException : Exception
{
    public string Handler { get; }

    public HandlerAlreadyInUseException(string handler)
    {
        Handler = handler;
    }

    protected HandlerAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Handler = string.Empty;
    }
}
