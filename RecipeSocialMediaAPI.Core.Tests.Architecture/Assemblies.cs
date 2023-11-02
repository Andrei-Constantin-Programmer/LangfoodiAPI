using System.Reflection;

namespace RecipeSocialMediaAPI.Core.Tests.Architecture;

internal static class Assemblies
{
    public static readonly Assembly DomainAssembly = typeof(Domain.AssemblyReference).Assembly;
    public static readonly Assembly ApplicationAssembly = typeof(Application.AssemblyReference).Assembly;
    public static readonly Assembly DataAccessAssembly = typeof(DataAccess.AssemblyReference).Assembly;
    public static readonly Assembly CoreAssembly = typeof(DataAccess.AssemblyReference).Assembly;
    
    public const string DOMAIN = "RecipeSocialMediaAPI.Domain";
    public const string APPLICATION = "RecipeSocialMediaAPI.Application";
    public const string DATA_ACCESS = "RecipeSocialMediaAPI.DataAccess";
    public const string CORE = "RecipeSocialMediaAPI.Core";
}
