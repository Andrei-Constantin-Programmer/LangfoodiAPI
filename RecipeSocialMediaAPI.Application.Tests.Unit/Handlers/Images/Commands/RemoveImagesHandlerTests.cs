using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Images.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Images;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Images.Commands;
public class RemoveImagesHandlerTests
{
    private readonly Mock<IImageHostingPersistenceRepository> _imageHostingPersistenceRepositoryMock;
    
    private readonly RemoveImagesHandler _removeImagesHandlerSUT;

    private readonly List<string> _test_public_ids = new() { "id1", "id2", "id3" };

    public RemoveImagesHandlerTests()
    {
        _imageHostingPersistenceRepositoryMock = new Mock<IImageHostingPersistenceRepository>();
        _removeImagesHandlerSUT = new RemoveImagesHandler(_imageHostingPersistenceRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RemoveImagesWorks_TaskIsCompleted()
    {
        // Given
        _imageHostingPersistenceRepositoryMock
            .Setup(x => x.BulkRemoveHostedImages(_test_public_ids))
            .Returns(true);

        // When
        var action = async () => await _removeImagesHandlerSUT
            .Handle(new RemoveImagesCommand(_test_public_ids), CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();
        _imageHostingPersistenceRepositoryMock
            .Verify(repo => repo.BulkRemoveHostedImages(_test_public_ids));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.IMAGE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_RemoveImagesFails_ExceptionThrown()
    {
        // Given
        _imageHostingPersistenceRepositoryMock
            .Setup(x => x.BulkRemoveHostedImages(_test_public_ids))
            .Returns(false);

        // When
        var action = async () => await _removeImagesHandlerSUT
            .Handle(new RemoveImagesCommand(_test_public_ids), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Could not remove images or only partially removed some images");
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
            .Handle(new RemoveImagesCommand(new()), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Could not remove images or only partially removed some images");
    }
}
