namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
internal class CorruptedMessageException : Exception
{
    public CorruptedMessageException(string? message) : base(message)
    {
    }
}