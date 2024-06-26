﻿using FluentAssertions;
using NetArchTest.Rules;
using RecipeSocialMediaAPI.Presentation.Tests.Architecture.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Presentation.Tests.Architecture.ArchitectureTests;

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
            .HaveDependencyOnAny(Assemblies.INFRASTRUCTURE, Assemblies.PRESENTATION)
            .GetResult();

        // Then
        testResult.IsSuccessful.Should().BeTrue();
    }
}
