using RecipeSocialMediaAPI.Domain.Utilities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RecipeSocialMediaAPI.Application.Tests.Unit")]
[assembly: InternalsVisibleTo("RecipeSocialMediaAPI.Application.Tests.Integration")]
namespace RecipeSocialMediaAPI.Application.Utilities;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}
