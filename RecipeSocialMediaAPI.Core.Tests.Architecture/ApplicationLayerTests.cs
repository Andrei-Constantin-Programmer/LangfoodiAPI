using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Architecture;

public class ApplicationLayerTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void ApplicationLayer_ShouldNotHaveDependenciesOnInfrastructureAndPresentation()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Assemblies.DATA_ACCESS, Assemblies.CORE)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }
}
