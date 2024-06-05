using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ConnectionUpdateException : Exception
{
    public ConnectionUpdateException(string message) : base(message) { }

    protected ConnectionUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
