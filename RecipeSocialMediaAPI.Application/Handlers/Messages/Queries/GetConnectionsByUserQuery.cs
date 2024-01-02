using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConnectionsByUserQuery(string UserId) : IRequest<IEnumerable<ConnectionDTO>>;

internal class GetConnectionsByUserHandler : IRequestHandler<GetConnectionsByUserQuery, IEnumerable<ConnectionDTO>>
{
    public Task<IEnumerable<ConnectionDTO>> Handle(GetConnectionsByUserQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
