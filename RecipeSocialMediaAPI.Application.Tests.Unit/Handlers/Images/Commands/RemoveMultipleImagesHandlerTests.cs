using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Images.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Images;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Images.Commands;
public class RemoveMultipleImagesHandlerTests
{
    private readonly Mock<IImageHostingPersistenceRepository> _imageHostingPersistenceRepositoryMock;
    
    private readonly RemoveMultipleImagesHandler _removeImagesHandlerSUT;

    private readonly List<string> _testPublicIds = new() { "id1", "id2", "id3" };

    public RemoveMultipleImagesHandlerTests()
    {
        _imageHostingPersistenceRepositoryMock = new Mock<IImageHostingPersistenceRepository>();
        _removeImagesHandlerSUT = new RemoveMultipleImagesHandler(_imageHostingPersistenceRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RemoveImagesWorks_TaskIsCompleted()
    {
        // Given
        _imageHostingPersistenceRepositoryMock
            .Setup(x => x.BulkRemoveHostedImages(_testPublicIds))
            .Returns(true);

        // When
        var action = async () => await _removeImagesHandlerSUT
            .Handle(new RemoveMultipleImagesCommand(_testPublicIds), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();
        _imageHostingPersistenceRepositoryMock
            .Verify(repo => repo.BulkRemoveHostedImages(_testPublicIds));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RemoveImagesFails_ExceptionThrown()
    {
        // Given
        _imageHostingPersistenceRepositoryMock
            .Setup(x => x.BulkRemoveHostedImages(_testPublicIds))
            .Returns(false);

        // When
        var action = async () => await _removeImagesHandlerSUT
            .Handle(new RemoveMultipleImagesCommand(_testPublicIds), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage($"Could not remove images or only partially removed some images from: [{string.Join(",", _testPublicIds)}]");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_EmptyIdsListAndRemoveImagesFails_ExceptionThrown()
    {
        // Given
        _imageHostingPersistenceRepositoryMock
            .Setup(x => x.BulkRemoveHostedImages(It.IsAny<List<string>>()))
            .Returns(false);

        // When
        var action = async () => await _removeImagesHandlerSUT
            .Handle(new RemoveMultipleImagesCommand(new()), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage($"Could not remove images or only partially removed some images from: []");
    }
}
