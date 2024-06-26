﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Images.Commands;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Images.Commands;

public class RemoveImageHandlerTests
{
    private readonly Mock<ICloudinaryWebClient> _cloudinaryWebClientMock;

    private readonly RemoveImageHandler _removeImageHandlerSUT;

    private const string TEST_PUBLIC_ID = "354234535sgf45";

    public RemoveImageHandlerTests() 
    {
        _cloudinaryWebClientMock = new Mock<ICloudinaryWebClient>();
        _removeImageHandlerSUT = new RemoveImageHandler(_cloudinaryWebClientMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RemoveImageWorks_TaskIsCompleted()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.RemoveHostedImage(TEST_PUBLIC_ID))
            .Returns(true);

        // When
        var action = async () => await _removeImageHandlerSUT
            .Handle(new RemoveImageCommand(TEST_PUBLIC_ID), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();
        _cloudinaryWebClientMock
            .Verify(repo => repo.RemoveHostedImage(TEST_PUBLIC_ID));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RemoveImageFails_ExceptionThrown()
    {
        // Given
        _cloudinaryWebClientMock
            .Setup(x => x.RemoveHostedImage(TEST_PUBLIC_ID))
            .Returns(false);

        // When
        var action = async () => await _removeImageHandlerSUT
            .Handle(new RemoveImageCommand(TEST_PUBLIC_ID), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<ImageRemovalException>()
            .WithMessage($"Could not remove image with publicId: {TEST_PUBLIC_ID}");
    }
}
