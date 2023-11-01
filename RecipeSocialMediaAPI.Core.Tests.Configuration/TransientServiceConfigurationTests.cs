using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Cryptography;
using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Services;

namespace RecipeSocialMediaAPI.Core.Tests.Configuration;

public class TransientServiceConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TransientServiceConfigurationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void CryptoService_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var cryptoService = scope.ServiceProvider.GetService(typeof(ICryptoService)) as CryptoService;

        // Then
        cryptoService.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MessageFactory_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var messageFactory = scope.ServiceProvider.GetService(typeof(IMessageFactory)) as MessageFactory;

        // Then
        messageFactory.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void UserFactory_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var userFactory = scope.ServiceProvider.GetService(typeof(IUserFactory)) as UserFactory;

        // Then
        userFactory.Should().NotBeNull();
    }
}
