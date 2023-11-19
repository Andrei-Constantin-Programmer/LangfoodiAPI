using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class UpdateConnectionHandlerTests
{
    private readonly UpdateConnectionHandler _updateConnectionHandlerSUT;

    public UpdateConnectionHandlerTests()
    {
        _updateConnectionHandlerSUT = new();
    }
}
