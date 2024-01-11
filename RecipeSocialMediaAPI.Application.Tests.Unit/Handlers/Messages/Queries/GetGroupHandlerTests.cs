using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetGroupHandlerTests
{
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;

    private readonly GetGroupHandler _getGroupHandlerSUT;

    public GetGroupHandlerTests()
    {
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();

        _getGroupHandlerSUT = new(_groupQueryRepositoryMock.Object);
    }
}
