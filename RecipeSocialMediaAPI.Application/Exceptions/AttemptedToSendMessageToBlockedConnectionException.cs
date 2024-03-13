namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class AttemptedToSendMessageToBlockedConnectionException : Exception
{
    public AttemptedToSendMessageToBlockedConnectionException(string message) : base(message) { }
}
