using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Configuration.InversionOfControlContainer;

public class IoCContainerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IServiceProvider _serviceProvider;

    public IoCContainerTests(WebApplicationFactory<Program> factory)
    {
        _serviceProvider = factory.Services;
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MongoConfiguration_ShouldBeConfiguredCorrectly()
    {
        // Given
        var mongoConfiguration = _serviceProvider.GetService(typeof(MongoDatabaseConfiguration)) as MongoDatabaseConfiguration;

        // Then
        mongoConfiguration.Should().NotBeNull();
        mongoConfiguration!.MongoConnectionString.Should().NotBeNullOrWhiteSpace();
        mongoConfiguration!.MongoDatabaseName.Should().NotBeNullOrWhiteSpace();
    }


}
