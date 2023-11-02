using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Reflection;

namespace RecipeSocialMediaAPI.Core.Tests.Architecture;

public class CleanArchitectureTests
{
    private readonly Assembly _domainAssembly;
    private readonly Assembly _applicationAssembly;
    private readonly Assembly _dataAccessAssembly;
    private readonly Assembly _coreAssembly;

    private const string DOMAIN = "RecipeSocialMediaAPI.Domain";
    private const string APPLICATION = "RecipeSocialMediaAPI.Application";
    private const string DATA_ACCESS = "RecipeSocialMediaAPI.DataAccess";
    private const string CORE = "RecipeSocialMediaAPI.Core";

    public CleanArchitectureTests()
    {
        _domainAssembly = typeof(Domain.AssemblyReference).Assembly;
        _applicationAssembly = typeof(Application.AssemblyReference).Assembly;
        _dataAccessAssembly = typeof(DataAccess.AssemblyReference).Assembly;
        _coreAssembly = typeof(Core.AssemblyReference).Assembly;
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void DomainLayer_ShouldNotHaveAnyDependencies()
    {
        // When
        var testResult = Types
            .InAssembly(_domainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(APPLICATION, DATA_ACCESS, CORE)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }
}
