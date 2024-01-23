using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConversationByGroupQuery(string GroupId) : IRequest<GroupConversationDTO>;

internal class GetConversationByGroupHandler : IRequestHandler<GetConversationByGroupQuery, GroupConversationDTO>
{
    public Task<GroupConversationDTO> Handle(GetConversationByGroupQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
