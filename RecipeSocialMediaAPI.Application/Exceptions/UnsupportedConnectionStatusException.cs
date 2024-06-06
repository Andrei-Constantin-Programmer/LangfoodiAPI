using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UnsupportedConnectionStatusException : Exception
{
    private const string UNSUPPORTED_STATUS_PROPERTY_NAME = "UnsupportedStatus";
    public string UnsupportedStatus { get; }

    public UnsupportedConnectionStatusException(string connectionStatus) : base($"Could not map {connectionStatus} to {typeof(ConnectionStatus)}")
    {
        UnsupportedStatus = connectionStatus;
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected UnsupportedConnectionStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        UnsupportedStatus = info.GetString(UNSUPPORTED_STATUS_PROPERTY_NAME) ?? string.Empty;
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(UNSUPPORTED_STATUS_PROPERTY_NAME, UnsupportedStatus);
    }
}
