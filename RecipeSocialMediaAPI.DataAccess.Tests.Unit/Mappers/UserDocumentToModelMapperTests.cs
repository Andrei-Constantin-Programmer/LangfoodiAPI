using Moq;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Mappers;

public class UserDocumentToModelMapperTests
{
    private readonly Mock<IUserFactory> _userFactoryMock;

    private readonly UserDocumentToModelMapper _userDocumentToModelMapperSUT;

    public UserDocumentToModelMapperTests()
    {
        _userFactoryMock = new Mock<IUserFactory>();

        _userDocumentToModelMapperSUT = new(_userFactoryMock.Object);
    }
}
