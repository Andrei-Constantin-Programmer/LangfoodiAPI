using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class DeleteGroupHandlerTests
{
    private readonly DeleteGroupHandler _deleteGroupHandlerSUT;

    public DeleteGroupHandlerTests()
    {
        _deleteGroupHandlerSUT = new();
    }
}
