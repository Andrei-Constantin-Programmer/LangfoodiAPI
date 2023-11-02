using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Architecture;

public class DomainLayerTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void DomainLayer_ShouldNotHaveAnyDependenciesOnOtherProjects()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Assemblies.APPLICATION, Assemblies.DATA_ACCESS, Assemblies.CORE)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }
}
