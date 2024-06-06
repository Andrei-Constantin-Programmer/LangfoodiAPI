using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class HandleAlreadyInUseException : Exception
{
    private const string HANDLE_PROPERTY_NAME = "Handle";
    public string Handle { get; }

    public HandleAlreadyInUseException(string handle)
    {
        Handle = handle;
    }

    protected HandleAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Handle = info.GetString(HANDLE_PROPERTY_NAME) ?? string.Empty;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(HANDLE_PROPERTY_NAME, Handle);
    }
}
