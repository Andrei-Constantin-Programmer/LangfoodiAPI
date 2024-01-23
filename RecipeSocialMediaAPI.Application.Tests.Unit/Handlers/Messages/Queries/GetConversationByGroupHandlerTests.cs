using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetConversationByGroupHandlerTests
{
    private readonly GetConversationByGroupHandler _getConversationByGroupHandlerSUT;

    public GetConversationByGroupHandlerTests()
    {
        _getConversationByGroupHandlerSUT = new();
    }
}
