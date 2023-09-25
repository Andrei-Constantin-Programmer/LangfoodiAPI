namespace RecipeSocialMediaAPI.Application.Utilities.Interfaces;

public interface IDateTimeProvider
{
    DateTimeOffset Now { get; }
}
