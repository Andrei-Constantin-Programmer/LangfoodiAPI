namespace RecipeSocialMediaAPI.Domain.Utilities;

public interface IDateTimeProvider
{
    DateTimeOffset Now { get; }
}
