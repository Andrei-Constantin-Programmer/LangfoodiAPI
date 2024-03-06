using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.Core.Tests.Architecture.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Reflection;

namespace RecipeSocialMediaAPI.Core.Tests.Architecture.ArchitectureTests;

public class GlobalTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.ARCHITECTURE)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void Exceptions_ShouldBePublicAndFormattedCorrectly()
    {
        // When
        var testResult = Types
            .InAssemblies(new List<Assembly>() 
            {
                Assemblies.DomainAssembly, 
                Assemblies.ApplicationAssembly,
                Assemblies.DataAccessAssembly,
                Assemblies.CoreAssembly
            })
            .That()
            .Inherit(typeof(Exception))
            .Should()
            .HaveNameMatching(".*Exception.*")
            .And()
            .BePublic()
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }
}
