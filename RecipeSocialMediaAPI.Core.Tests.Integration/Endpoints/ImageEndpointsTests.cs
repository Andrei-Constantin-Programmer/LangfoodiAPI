using CloudinaryDotNet;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;
public class ImageEndpointsTests : EndpointTestBase
{
    private readonly CloudinarySignatureDTO _signatureTestData = new("signature1", new DateTimeOffset(2023, 08, 19, 12, 30, 0, TimeSpan.Zero).ToUnixTimeSeconds());

    public ImageEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) {}

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetCloudinarySignature_SignatureGenerated_ReturnGeneratedCloudinarySignature()
    {
        // Given
        _cloudinarySignatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), null))
            .Returns(_signatureTestData);

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
    public async void GetCloudinarySignature_SignatureNotGenerated_ReturnNotOk()
    {
        // Given
        _cloudinarySignatureServiceMock
            .Setup(x => x.GenerateSignature(It.IsAny<Cloudinary>(), It.IsAny<string>()))
            .Returns((CloudinarySignatureDTO?)null);

        // When
        var result = await _client.PostAsync("image/get/cloudinary-signature", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void ImageSingleDelete_GivenPublicIdAndNoError_ReturnOk()
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
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void ImageSingleDelete_GivenPublicIdAndError_ReturnNotOk()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.RemoveHostedImage(
                It.IsAny<CloudinarySignatureDTO>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(false);

        // When
        var result = await _client.DeleteAsync("/image/single-delete?publicId=publicId123");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void ImageBulkDelete_GivenPublicIdsAndNoError_ReturnOk()
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
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void ImageBulkDelete_GivenPublicIdsAndError_ReturnNotOk()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.BulkRemoveHostedImages(
                It.IsAny<List<string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(false);

        // When
        var result = await _client.DeleteAsync("/image/bulk-delete?publicIds=publicId1&publicIds=publicId2&publicIds=publicId3");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
