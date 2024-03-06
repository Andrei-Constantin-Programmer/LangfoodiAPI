using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.Presentation.Tests.Architecture.TestHelpers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Presentation.Tests.Architecture.ArchitectureTests;

public class DataAccessLayerTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void DataAccessLayer_ShouldNotHaveDependenciesOnPresentation()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.DataAccessAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Assemblies.PRESENTATION)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void DataAccessLayer_RepositoriesShouldDependOnBothApplicationAndDomain()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.DataAccessAssembly)
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
    public void DataAccessLayer_MongoDocumentsShouldInheritFromMongoDocumentClass()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.DataAccessAssembly)
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
    public void DataAccessLayer_RepositoryInterfacesShouldNotExist()
    {
        // When
        var testResult = Types
            .InAssembly(Assemblies.DataAccessAssembly)
            .That()
            .AreInterfaces()
            .And()
            .HaveNameEndingWith("Repository");

        // Then
        testResult.GetTypes().Should().BeEmpty();
    }
}
