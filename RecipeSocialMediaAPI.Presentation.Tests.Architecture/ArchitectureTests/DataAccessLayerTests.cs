using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.Presentation.Tests.Architecture.TestHelpers;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Presentation.Tests.Architecture.ArchitectureTests;

public class InfrastructureLayerTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void InfrastructureLayer_ShouldNotHaveDependenciesOnPresentation()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Assemblies.PRESENTATION)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void InfrastructureLayer_RepositoriesShouldDependOnBothApplicationAndDomain()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.InfrastructureAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .And()
            .DoNotHaveNameStartingWith("ImageHosting")
            .Should()
            .HaveDependencyOnAll(Assemblies.APPLICATION, Assemblies.DOMAIN)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void InfrastructureLayer_MongoDocumentsShouldInheritFromMongoDocumentClass()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.InfrastructureAssembly)
            .That()
            .HaveNameEndingWith("Document")
            .And()
            .DoNotHaveName(nameof(MongoDocument))
            .Should()
            .Inherit(typeof(MongoDocument))
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void InfrastructureLayer_RepositoryInterfacesShouldNotExist()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.InfrastructureAssembly)
            .That()
            .AreInterfaces()
            .And()
            .HaveNameEndingWith("Repository");

        // Then
        testResult.GetTypes().Should().BeEmpty();
    }
}
