using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.Core.Tests.Architecture.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Architecture.ArchitectureTests;

public class DataAccessLayerTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DataAccessLayer_ShouldNotHaveDependenciesOnCore()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.DataAccessAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Assemblies.CORE)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }
}
