using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.TestInfrastructure;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using FluentAssertions;

namespace RecipeSocialMediaAPI.Core.Tests.Configuration.InversionOfControlContainer;

public class OptionConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IServiceProvider _serviceProvider;

    public OptionConfigurationTests(WebApplicationFactory<Program> factory)
    {
        _serviceProvider = factory.Services;
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MongoDatabaseOptions_ShouldBeConfiguredCorrectly()
    {
        // Given
        var mongoOptions = _serviceProvider.GetService(typeof(IOptions<MongoDatabaseOptions>)) as IOptions<MongoDatabaseOptions>;

        // Then
        mongoOptions.Should().NotBeNull();
        mongoOptions!.Value.Should().NotBeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void CloudinaryApiOptions_ShouldBeConfiguredCorrectly()
    {
        // Given
        var cloudinaryOptions = _serviceProvider.GetService(typeof(IOptions<CloudinaryApiOptions>)) as IOptions<CloudinaryApiOptions>;

        // Then
        cloudinaryOptions.Should().NotBeNull();
        cloudinaryOptions!.Value.Should().NotBeNull();
    }
}
