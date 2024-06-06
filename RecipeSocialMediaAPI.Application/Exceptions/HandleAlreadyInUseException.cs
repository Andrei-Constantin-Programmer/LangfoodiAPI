using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class HandleAlreadyInUseException : Exception
{
    public string Handle { get; }

    public HandleAlreadyInUseException(string handler)
    {
        Handle = handler;
    }

    protected HandleAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Handle = info.GetString("Handler") ?? string.Empty;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("Handle", Handle);
    }
}
