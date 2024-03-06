using CloudinaryDotNet;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.X86;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;
public class ImageEndpointsTests : EndpointTestBase
{
    private readonly CloudinarySignatureDTO _signatureTestData = new("signature1", new DateTimeOffset(2023, 08, 19, 12, 30, 0, TimeSpan.Zero).ToUnixTimeSeconds());

    public ImageEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) {}

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task GetCloudinarySignature_SignatureGenerated_ReturnGeneratedCloudinarySignature()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUser($"handle_1", "UserName 1", "email1@mail.com", _fakeCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _cloudinarySignatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), null))
        .Returns(_signatureTestData);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync("image/get/cloudinary-signature", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        CloudinarySignatureDTO generatedSignature = (await result.Content.ReadFromJsonAsync<CloudinarySignatureDTO>())!;

        generatedSignature.Signature.Should().Be(_signatureTestData.Signature);
        generatedSignature.TimeStamp.Should().Be(_signatureTestData.TimeStamp);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task GetCloudinarySignature_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        _cloudinarySignatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), null))
        .Returns(_signatureTestData);

        // When
        var result = await _client.PostAsync("image/get/cloudinary-signature", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task ImageSingleDelete_GivenPublicIdAndNoError_ReturnOk()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUser($"handle_1", "UserName 1", "email1@mail.com", _fakeCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _cloudinaryWebClientMock
            .Setup(x => x.RemoveHostedImage(
                It.IsAny<CloudinarySignatureDTO>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(true);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync("/image/single-delete?publicId=publicId123");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task ImageSingleDelete_GivenPublicIdAndError_ReturnNotOk()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUser($"handle_1", "UserName 1", "email1@mail.com", _fakeCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _cloudinaryWebClientMock
            .Setup(x => x.RemoveHostedImage(
                It.IsAny<CloudinarySignatureDTO>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(false);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync("/image/single-delete?publicId=publicId123");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task ImageSingleDelete_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.RemoveHostedImage(
                It.IsAny<CloudinarySignatureDTO>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(true);

        // When
        var result = await _client.DeleteAsync("/image/single-delete?publicId=publicId123");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task ImageBulkDelete_GivenPublicIdsAndNoError_ReturnOk()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUser($"handle_1", "UserName 1", "email1@mail.com", _fakeCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(
                It.IsAny<List<string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(true);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync("/image/bulk-delete?publicIds=publicId1&publicIds=publicId2&publicIds=publicId3");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task ImageBulkDelete_GivenPublicIdsAndError_ReturnNotOk()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUser($"handle_1", "UserName 1", "email1@mail.com", _fakeCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(
                It.IsAny<List<string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(false);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync("/image/bulk-delete?publicIds=publicId1&publicIds=publicId2&publicIds=publicId3");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task ImageBulkDelete_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(
                It.IsAny<List<string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(true);

        // When
        var result = await _client.DeleteAsync("/image/bulk-delete?publicIds=publicId1&publicIds=publicId2&publicIds=publicId3");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
