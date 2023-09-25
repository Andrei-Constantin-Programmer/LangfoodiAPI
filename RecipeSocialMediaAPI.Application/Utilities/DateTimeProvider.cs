using RecipeSocialMediaAPI.Application.Utilities.Interfaces;

namespace RecipeSocialMediaAPI.Application.Utilities;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}
