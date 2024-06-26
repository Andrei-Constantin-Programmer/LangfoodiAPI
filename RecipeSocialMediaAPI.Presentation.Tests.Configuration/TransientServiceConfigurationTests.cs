﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RecipeSocialMediaAPI.Application.Cryptography;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Services;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Application.WebClients;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Presentation.SignalR;
using RecipeSocialMediaAPI.TestInfrastructure;

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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void HttpClientFactory_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var signatureService = scope.ServiceProvider.GetService(typeof(IHttpClientFactory));

        // Then
        signatureService.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void PasswordCryptoService_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var passwordCryptoService = scope.ServiceProvider.GetService(typeof(IPasswordCryptoService)) as PasswordCryptoService;

        // Then
        passwordCryptoService.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void DataCryptoService_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var dataCryptoService = scope.ServiceProvider.GetService(typeof(IDataCryptoService)) as DataCryptoService;

        // Then
        dataCryptoService.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void UserFactory_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var userFactory = scope.ServiceProvider.GetService(typeof(IUserFactory)) as UserFactory;

        // Then
        userFactory.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void CloudinaryWebClient_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var webClient = scope.ServiceProvider.GetService(typeof(ICloudinaryWebClient)) as CloudinaryWebClient;

        // Then
        webClient.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void MessageNotificationService_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var messageNotificationService = scope.ServiceProvider.GetService(typeof(IMessageNotificationService)) as MessageNotificationService;

        // Then
        messageNotificationService.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void BearerTokenService_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var bearerTokenService = scope.ServiceProvider.GetService(typeof(IBearerTokenGeneratorService)) as BearerTokenGeneratorService;

        // Then
        bearerTokenService.Should().NotBeNull();
    }
}
