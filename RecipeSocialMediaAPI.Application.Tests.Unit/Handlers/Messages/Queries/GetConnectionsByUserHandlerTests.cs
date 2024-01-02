using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetConnectionsByUserHandlerTests
{
    private readonly GetConnectionsByUserHandler _getConnectionsByUserHandlerSUT;

    public GetConnectionsByUserHandlerTests()
    {
        _getConnectionsByUserHandlerSUT = new();
    }
}
