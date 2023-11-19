using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConnectionQuery(string UserId1, string UserId2) : IRequest<ConnectionDTO>;

internal class GetConnectionHandler : IRequestHandler<GetConnectionQuery, ConnectionDTO>
{
    public Task<ConnectionDTO> Handle(GetConnectionQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
