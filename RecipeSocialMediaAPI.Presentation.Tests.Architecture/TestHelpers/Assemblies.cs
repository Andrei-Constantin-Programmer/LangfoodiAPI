using System.Reflection;

namespace RecipeSocialMediaAPI.Presentation.Tests.Architecture.TestHelpers;

internal static class Assemblies
{
    public static readonly Assembly DomainAssembly = typeof(Domain.AssemblyReference).Assembly;
    public static readonly Assembly ApplicationAssembly = typeof(Application.AssemblyReference).Assembly;
    public static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.AssemblyReference).Assembly;
    public static readonly Assembly PresentationAssembly = typeof(AssemblyReference).Assembly;

    public const string DOMAIN = "RecipeSocialMediaAPI.Domain";
    public const string APPLICATION = "RecipeSocialMediaAPI.Application";
    public const string INFRASTRUCTURE = "RecipeSocialMediaAPI.Infrastructure";
    public const string PRESENTATION = "RecipeSocialMediaAPI.Presentation";
}
