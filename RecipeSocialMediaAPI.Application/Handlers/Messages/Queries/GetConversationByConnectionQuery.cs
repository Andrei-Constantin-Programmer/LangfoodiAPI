using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConversationByConnectionQuery(string ConnectionId) : IRequest<ConnectionConversationDTO>;

internal class GetConversationByConnectionHandler : IRequestHandler<GetConversationByConnectionQuery, ConnectionConversationDTO>
{
    public Task<ConnectionConversationDTO> Handle(GetConversationByConnectionQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
