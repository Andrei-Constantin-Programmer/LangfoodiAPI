namespace RecipeSocialMediaAPI.Core.Utilities
{
    internal interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
    }
}
