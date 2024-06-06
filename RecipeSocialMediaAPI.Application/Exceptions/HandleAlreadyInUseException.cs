using System.Diagnostics.CodeAnalysis;
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

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected HandleAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Handle = info.GetString(HANDLE_PROPERTY_NAME) ?? string.Empty;
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(HANDLE_PROPERTY_NAME, Handle);
    }
}
