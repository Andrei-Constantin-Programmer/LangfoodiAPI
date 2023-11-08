namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class CorruptedMessageException : Exception
{
    public CorruptedMessageException(string? message) : base(message)
    {
    }
}