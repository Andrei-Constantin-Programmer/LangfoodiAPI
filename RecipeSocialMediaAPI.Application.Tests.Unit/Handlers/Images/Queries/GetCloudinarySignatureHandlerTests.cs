using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Handlers.Images.Queries;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Images.Queries;

public class GetCloudinarySignatureHandlerTests
{
    private readonly Mock<ICloudinaryWebClient> _cloudinaryWebClientMock;

    private readonly GetCloudinarySignatureHandler _getCloudinarySignatureHandlerSUT;

    public GetCloudinarySignatureHandlerTests()
    {
        _cloudinaryWebClientMock = new Mock<ICloudinaryWebClient>();
        _getCloudinarySignatureHandlerSUT = new GetCloudinarySignatureHandler(_cloudinaryWebClientMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_GenerateSignatureWorks_ReturnSignatureDTO()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.GenerateSignature(null))
            .Returns(new CloudinarySignatureDto("sig", 1000));

        // When
        var result = await _getCloudinarySignatureHandlerSUT.Handle(new GetCloudinarySignatureQuery(), CancellationToken.None);

        // Then
        result.Signature.Should().Be("sig");
        result.TimeStamp.Should().Be(1000);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_NoSignatureGenerated_ThrowsException()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.GenerateSignature(null))
            .Returns((CloudinarySignatureDto?)null);

        // When
        var action = async () => await _getCloudinarySignatureHandlerSUT.Handle(new GetCloudinarySignatureQuery(), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Failed to generate signature");
    }
}
