using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.TestInfrastructure;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using FluentAssertions;
using RecipeSocialMediaAPI.Core.Options;
using RecipeSocialMediaAPI.Core.OptionValidation;
using RecipeSocialMediaAPI.Application.Options;

namespace RecipeSocialMediaAPI.Core.Tests.Configuration;

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

        // When
        var validationResult = new MongoDatabaseOptionValidator().Validate(mongoOptions!.Value).IsValid;

        // Then
        mongoOptions.Should().NotBeNull();
        mongoOptions!.Value.Should().NotBeNull();
        validationResult.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void CloudinaryApiOptions_ShouldBeConfiguredCorrectly()
    {
        // Given
        var cloudinaryOptions = _serviceProvider.GetService(typeof(IOptions<CloudinaryApiOptions>)) as IOptions<CloudinaryApiOptions>;

        // When
        var validationResult = new CloudinaryApiOptionValidator().Validate(cloudinaryOptions!.Value).IsValid;

        // Then
        cloudinaryOptions.Should().NotBeNull();
        cloudinaryOptions!.Value.Should().NotBeNull();
        validationResult.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void DataDogOptions_ShouldBeConfiguredCorrectly()
    {
        // Given
        var dataDogOptions = _serviceProvider.GetService(typeof(IOptions<DataDogOptions>)) as IOptions<DataDogOptions>;

        // When
        var validationResult = new DataDogOptionValidator().Validate(dataDogOptions!.Value).IsValid;

        // Then
        dataDogOptions.Should().NotBeNull();
        dataDogOptions!.Value.Should().NotBeNull();
        validationResult.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void JwtOptions_ShouldBeConfiguredCorrectly()
    {
        // Given
        var jwtOptions = _serviceProvider.GetService(typeof(IOptions<JwtOptions>)) as IOptions<JwtOptions>;

        // When
        var validationResult = new JwtOptionValidator().Validate(jwtOptions!.Value).IsValid;

        // Then
        jwtOptions.Should().NotBeNull();
        jwtOptions!.Value.Should().NotBeNull();
        validationResult.Should().BeTrue();
    }
}
