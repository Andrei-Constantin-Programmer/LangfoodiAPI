using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Application.Utilities;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}
