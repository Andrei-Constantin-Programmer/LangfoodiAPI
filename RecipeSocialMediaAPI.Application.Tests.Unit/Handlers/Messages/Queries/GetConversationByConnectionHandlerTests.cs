using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetConversationByConnectionHandlerTests
{
    private readonly GetConversationByConnectionHandler _getConversationByConnectionHandlerSUT;

    public GetConversationByConnectionHandlerTests()
    {
        _getConversationByConnectionHandlerSUT = new();
    }
}
