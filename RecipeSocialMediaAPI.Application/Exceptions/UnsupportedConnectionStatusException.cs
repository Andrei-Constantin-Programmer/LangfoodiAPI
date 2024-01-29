using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UnsupportedConnectionStatusException : Exception
{
    public string UnsupportedStatus { get; }
    public UnsupportedConnectionStatusException(string connectionStatus) : base($"Could not map {connectionStatus} to {typeof(ConnectionStatus)}") 
    {
        UnsupportedStatus = connectionStatus;
    }
}
