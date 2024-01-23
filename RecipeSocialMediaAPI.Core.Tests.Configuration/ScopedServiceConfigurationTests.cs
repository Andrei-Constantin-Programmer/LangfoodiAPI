using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.DataAccess.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Repositories.Users;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
using RecipeSocialMediaAPI.DataAccess.Repositories.ImageHosting;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.Application.Repositories.Images;
using RecipeSocialMediaAPI.DataAccess.Repositories.Images;

namespace RecipeSocialMediaAPI.Core.Tests.Configuration;

public class ScopedServiceConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ScopedServiceConfigurationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void UserDocumentToModelMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var userDocumentToModelMapper = scope.ServiceProvider.GetService(typeof(IUserDocumentToModelMapper)) as UserDocumentToModelMapper;

        // Then
        userDocumentToModelMapper.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void RecipeDocumentToModelMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var recipeDocumentToModelMapper = scope.ServiceProvider.GetService(typeof(IRecipeDocumentToModelMapper)) as RecipeDocumentToModelMapper;

        // Then
        recipeDocumentToModelMapper.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MessageDocumentToModelMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var messageDocumentToModelMapper = scope.ServiceProvider.GetService(typeof(IMessageDocumentToModelMapper)) as MessageDocumentToModelMapper;

        // Then
        messageDocumentToModelMapper.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void ConnectionDocumentToModelMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var connectionDocumentToModelMapper = scope.ServiceProvider.GetService(typeof(IConnectionDocumentToModelMapper)) as ConnectionDocumentToModelMapper;

        // Then
        connectionDocumentToModelMapper.Should().NotBeNull();
    }
    
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void GroupDocumentToModelMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var groupDocumentToModelMapper = scope.ServiceProvider.GetService(typeof(IGroupDocumentToModelMapper)) as GroupDocumentToModelMapper;

        // Then
        groupDocumentToModelMapper.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void RecipeQueryRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var recipeQueryRepository = scope.ServiceProvider.GetService(typeof(IRecipeQueryRepository)) as RecipeQueryRepository;

        // Then
        recipeQueryRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void RecipePersistenceRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var recipePersistenceRepository = scope.ServiceProvider.GetService(typeof(IRecipePersistenceRepository)) as RecipePersistenceRepository;

        // Then
        recipePersistenceRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MessageQueryRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var messageQueryRepository = scope.ServiceProvider.GetService(typeof(IMessageQueryRepository)) as MessageQueryRepository;

        // Then
        messageQueryRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MessagePersistenceRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var messagePersistenceRepository = scope.ServiceProvider.GetService(typeof(IMessagePersistenceRepository)) as MessagePersistenceRepository;

        // Then
        messagePersistenceRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void ConnectionQueryRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var connectionQueryRepository = scope.ServiceProvider.GetService(typeof(IConnectionQueryRepository)) as ConnectionQueryRepository;

        // Then
        connectionQueryRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void ConnectionPersistenceRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var connectionPersistenceRepository = scope.ServiceProvider.GetService(typeof(IConnectionPersistenceRepository)) as ConnectionPersistenceRepository;

        // Then
        connectionPersistenceRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void UserQueryRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var userQueryRepository = scope.ServiceProvider.GetService(typeof(IUserQueryRepository)) as UserQueryRepository;

        // Then
        userQueryRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void UserPersistenceRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var userPersistenceRepository = scope.ServiceProvider.GetService(typeof(IUserPersistenceRepository)) as UserPersistenceRepository;

        // Then
        userPersistenceRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void ImageHostingQueryRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var imageHostingQueryRepository = scope.ServiceProvider.GetService(typeof(IImageHostingQueryRepository)) as ImageHostingQueryRepository;

        // Then
        imageHostingQueryRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void ImageHostingPersistenceRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var imageHostingQueryRepository = scope.ServiceProvider.GetService(typeof(IImageHostingPersistenceRepository)) as ImageHostingPersistenceRepository;

        // Then
        imageHostingQueryRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MongoCollectionFactory_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var mongoCollectionFactory = scope.ServiceProvider.GetService(typeof(IMongoCollectionFactory)) as MongoCollectionFactory;

        // Then
        mongoCollectionFactory.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void ConversationDocumentToModelMapper_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var conversationDocumentToModelMapper = scope.ServiceProvider.GetService(typeof(IConversationDocumentToModelMapper)) as ConversationDocumentToModelMapper;

        // Then
        conversationDocumentToModelMapper.Should().NotBeNull();
    }
    
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void ConversationQueryRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var conversationQueryRepository = scope.ServiceProvider.GetService(typeof(IConversationQueryRepository)) as ConversationQueryRepository;

        // Then
        conversationQueryRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void ConversationPersistenceRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var conversationPersistenceRepository = scope.ServiceProvider.GetService(typeof(IConversationPersistenceRepository)) as ConversationPersistenceRepository;

        // Then
        conversationPersistenceRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void GroupPersistenceRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var groupPersistenceRepository = scope.ServiceProvider.GetService(typeof(IGroupPersistenceRepository)) as GroupPersistenceRepository;

        // Then
        groupPersistenceRepository.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void GroupQueryRepository_ShouldBeConfiguredCorrectly()
    {
        // Given
        using var scope = _factory.Services.CreateScope();
        var groupQueryRepository = scope.ServiceProvider.GetService(typeof(IGroupQueryRepository)) as GroupQueryRepository;

        // Then
        groupQueryRepository.Should().NotBeNull();
    }
}
