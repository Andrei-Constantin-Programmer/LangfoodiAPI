using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConnectionsByUserQuery(string UserId) : IRequest<IEnumerable<ConnectionDTO>>;

internal class GetConnectionsByUserHandler : IRequestHandler<GetConnectionsByUserQuery, IEnumerable<ConnectionDTO>>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IConnectionQueryRepository _connectionQueryRepository;

    public GetConnectionsByUserHandler(IUserQueryRepository userQueryRepository, IConnectionQueryRepository connectionQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
        _connectionQueryRepository = connectionQueryRepository;
    }

    public async Task<IEnumerable<ConnectionDTO>> Handle(GetConnectionsByUserQuery request, CancellationToken cancellationToken)
    {
        IUserAccount user = (await _userQueryRepository.GetUserById(request.UserId, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.UserId}");

        return (await _connectionQueryRepository
            .GetConnectionsForUser(user, cancellationToken))
            .Select(connection => new ConnectionDTO(
                connection.ConnectionId,
                connection.Account1.Id,
                connection.Account2.Id,
                connection.Status.ToString()));
    }
}
