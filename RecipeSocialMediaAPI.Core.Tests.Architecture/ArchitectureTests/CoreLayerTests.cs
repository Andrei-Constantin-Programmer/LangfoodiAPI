using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.Core.Tests.Architecture.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Architecture.ArchitectureTests;

public class CoreLayerTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void CoreLayer_EndpointsShouldNotHaveDependenciesOnDomainOrDataAccess()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.CoreAssembly)
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
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void CoreLayer_EndpointsShouldHaveDependencyOnMediatR()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.CoreAssembly)
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
