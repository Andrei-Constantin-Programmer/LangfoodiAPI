using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetMessagesByConversationHandlerTests
{
    private readonly GetMessagesByConversationHandler _getMessagesByConversationHandlerSUT;

    public GetMessagesByConversationHandlerTests()
    {
        _getMessagesByConversationHandlerSUT = new();
    }
}
