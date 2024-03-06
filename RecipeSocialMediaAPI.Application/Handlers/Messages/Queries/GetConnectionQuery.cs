using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConnectionQuery(string UserId1, string UserId2) : IRequest<ConnectionDTO>;

internal class GetConnectionHandler : IRequestHandler<GetConnectionQuery, ConnectionDTO>
{
    private readonly IConnectionQueryRepository _connectionQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public GetConnectionHandler(IConnectionQueryRepository connectionQueryRepository, IUserQueryRepository userQueryRepository)
    {
        _connectionQueryRepository = connectionQueryRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<ConnectionDTO> Handle(GetConnectionQuery request, CancellationToken cancellationToken)
    {
        IUserAccount user1 = (await _userQueryRepository.GetUserById(request.UserId1, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.UserId1}");
        IUserAccount user2 = (await _userQueryRepository.GetUserById(request.UserId2, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.UserId2}");

        IConnection connection = await _connectionQueryRepository.GetConnection(user1, user2, cancellationToken)
            ?? throw new ConnectionNotFoundException($"No connection found between users with ids {request.UserId1} and {request.UserId2}");

        return new ConnectionDTO(connection.ConnectionId, connection.Account1.Id, connection.Account2.Id, connection.Status.ToString());
    }
}
