using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Recipes;
using RecipeSocialMediaAPI.Application.Utilities;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Users;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Messages;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Services;

namespace RecipeSocialMediaAPI.Core.Tests.Configuration;

public class SingletonServiceConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IServiceProvider _serviceProvider;

    public SingletonServiceConfigurationTests(WebApplicationFactory<Program> factory)
    {
        _serviceProvider = factory.Services;
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void DateTimeProvider_ShouldBeConfiguredCorrectly()
    {
        // Given
        var dateTimeProvider = _serviceProvider.GetService(typeof(IDateTimeProvider)) as DateTimeProvider;

        // Then
        dateTimeProvider.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void UserValidationService_ShouldBeConfiguredCorrectly()
    {
        // Given
        var userValidationService = _serviceProvider.GetService(typeof(IUserValidationService)) as UserValidationService;

        // Then
        userValidationService.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void RecipeValidationService_ShouldBeConfiguredCorrectly()
    {
        // Given
        var recipeValidationService = _serviceProvider.GetService(typeof(IRecipeValidationService)) as RecipeValidationService;

        // Then
        recipeValidationService.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void RecipeMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        var recipeMapper = _serviceProvider.GetService(typeof(IRecipeMapper)) as RecipeMapper;

        // Then
        recipeMapper.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void UserMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        var userMapper = _serviceProvider.GetService(typeof(IUserMapper)) as UserMapper;

        // Then
        userMapper.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void MessageMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        var messageMapper = _serviceProvider.GetService(typeof(IMessageMapper)) as MessageMapper;

        // Then
        messageMapper.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void ConversationMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        var conversationMapper = _serviceProvider.GetService(typeof(IConversationMapper)) as ConversationMapper;

        // Then
        conversationMapper.Should().NotBeNull();
    }
}
