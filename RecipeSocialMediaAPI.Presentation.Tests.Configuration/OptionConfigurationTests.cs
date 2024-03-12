using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Application.Options;
using RecipeSocialMediaAPI.Core.OptionValidation;
using RecipeSocialMediaAPI.Infrastructure.Helpers;
using RecipeSocialMediaAPI.Presentation.Options;
using RecipeSocialMediaAPI.Presentation.OptionValidation;
using RecipeSocialMediaAPI.TestInfrastructure;

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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void CloudinaryOptions_ShouldBeConfiguredCorrectly()
    {
        // Given
        var cloudinaryOptions = _serviceProvider.GetService(typeof(IOptions<CloudinaryOptions>)) as IOptions<CloudinaryOptions>;

        // When
        var validationResult = new CloudinaryOptionValidator().Validate(cloudinaryOptions!.Value).IsValid;

        // Then
        cloudinaryOptions.Should().NotBeNull();
        cloudinaryOptions!.Value.Should().NotBeNull();
        validationResult.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.CONFIGURATION)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public void EncryptionOptions_ShouldBeConfiguredCorrectly()
    {
        // Given
        var encryptionOptions = _serviceProvider.GetService(typeof(IOptions<EncryptionOptions>)) as IOptions<EncryptionOptions>;

        // When
        var validationResult = new EncryptionOptionValidator().Validate(encryptionOptions!.Value).IsValid;

        // Then
        encryptionOptions.Should().NotBeNull();
        encryptionOptions!.Value.Should().NotBeNull();
        validationResult.Should().BeTrue();
    }
}
