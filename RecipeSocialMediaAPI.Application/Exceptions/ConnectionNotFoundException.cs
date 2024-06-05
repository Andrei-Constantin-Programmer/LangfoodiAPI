using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ConnectionNotFoundException : Exception
{
    public ConnectionNotFoundException(string message) : base(message) { }

    protected ConnectionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
