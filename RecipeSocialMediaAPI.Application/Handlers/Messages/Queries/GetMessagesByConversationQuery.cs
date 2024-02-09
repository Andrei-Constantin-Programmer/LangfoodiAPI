using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetMessagesByConversationQuery(string ConversationId) : IRequest<List<MessageDTO>>;

internal class GetMessagesByConversationHandler : IRequestHandler<GetMessagesByConversationQuery, List<MessageDTO>>
{
    public Task<List<MessageDTO>> Handle(GetMessagesByConversationQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
