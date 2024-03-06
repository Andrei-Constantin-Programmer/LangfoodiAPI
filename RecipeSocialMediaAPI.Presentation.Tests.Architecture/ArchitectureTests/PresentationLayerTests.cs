using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.Presentation.Tests.Architecture.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Presentation.Tests.Architecture.ArchitectureTests;

public class PresentationLayerTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void PresentationLayer_EndpointsShouldNotHaveDependenciesOnDomainOrDataAccess()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.PresentationAssembly)
            .That()
            .HaveNameEndingWith("Endpoints")
            .ShouldNot()
            .HaveDependencyOnAny(Assemblies.DOMAIN, Assemblies.DATA_ACCESS)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void PresentationLayer_EndpointsShouldHaveDependencyOnMediatR()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.PresentationAssembly)
            .That()
            .HaveNameEndingWith("Endpoints")
            .And()
            .DoNotHaveNameStartingWith("Test")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }
}
