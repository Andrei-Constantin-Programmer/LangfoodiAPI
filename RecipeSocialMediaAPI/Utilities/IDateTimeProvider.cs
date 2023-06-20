namespace RecipeSocialMediaAPI.Utilities
{
    internal interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
    }
}
