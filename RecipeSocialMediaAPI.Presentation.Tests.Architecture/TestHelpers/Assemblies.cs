using System.Reflection;

namespace RecipeSocialMediaAPI.Presentation.Tests.Architecture.TestHelpers;

internal static class Assemblies
{
    public static readonly Assembly DomainAssembly = typeof(Domain.AssemblyReference).Assembly;
    public static readonly Assembly ApplicationAssembly = typeof(Application.AssemblyReference).Assembly;
    public static readonly Assembly DataAccessAssembly = typeof(DataAccess.AssemblyReference).Assembly;
    public static readonly Assembly PresentationAssembly = typeof(AssemblyReference).Assembly;

    public const string DOMAIN = "RecipeSocialMediaAPI.Domain";
    public const string APPLICATION = "RecipeSocialMediaAPI.Application";
    public const string DATA_ACCESS = "RecipeSocialMediaAPI.DataAccess";
    public const string PRESENTATION = "RecipeSocialMediaAPI.Presentation";
}
