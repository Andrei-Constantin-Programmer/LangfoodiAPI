using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class EmailAlreadyInUseException : Exception
{
    private const string EMAIL_PROPERTY_NAME = "Email";
    public string Email { get; }

    public EmailAlreadyInUseException(string email)
    {
        Email = email;
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected EmailAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Email = info.GetString(EMAIL_PROPERTY_NAME) ?? string.Empty;
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(EMAIL_PROPERTY_NAME, Email);
    }
}
