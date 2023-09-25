using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RecipeSocialMediaAPI.Application.Tests.Unit")]
[assembly: InternalsVisibleTo("RecipeSocialMediaAPI.Application.Tests.Integration")]
namespace RecipeSocialMediaAPI.Application.Utilities.Interfaces;

public interface IDateTimeProvider
{
    DateTimeOffset Now { get; }
}
