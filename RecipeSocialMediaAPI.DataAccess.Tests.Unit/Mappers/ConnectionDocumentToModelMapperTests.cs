using Moq;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Mappers;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Mappers;

public class ConnectionDocumentToModelMapperTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly ConnectionDocumentToModelMapper _connectionDocumentToModelMapperSUT;

    public ConnectionDocumentToModelMapperTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _connectionDocumentToModelMapperSUT = new(_userQueryRepositoryMock.Object);
    }
}
