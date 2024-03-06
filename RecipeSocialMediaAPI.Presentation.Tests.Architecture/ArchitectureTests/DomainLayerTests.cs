using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.Presentation.Tests.Architecture.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Presentation.Tests.Architecture.ArchitectureTests;

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
            .HaveDependencyOnAny(Assemblies.APPLICATION, Assemblies.DATA_ACCESS, Assemblies.PRESENTATION)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void DomainLayer_ServicesShouldBeNamedCorrectly()
    {
        // Given
        var testResult = Types
            .InNamespace($"{Assemblies.DOMAIN}.Services")
            .Should()
            .HaveNameEndingWith("Service")
            .Or()
            .HaveNameEndingWith("Factory")
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void DomainLayer_ClassesShouldHaveNoNestedInheritance()
    {
        // Given
        var testResult = Types
            .InAssembly(Assemblies.DomainAssembly)
            .That()
            .AreClasses()
            .Should()
            .MeetCustomRule(new DoesNotHaveNestedInheritanceRule())
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }
}
