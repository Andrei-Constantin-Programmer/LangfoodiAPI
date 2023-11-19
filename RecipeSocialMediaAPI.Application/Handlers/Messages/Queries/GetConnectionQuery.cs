using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetConnectionQuery(string UserId1, string UserId2) : IRequest<ConnectionDTO?>;

internal class GetConnectionHandler : IRequestHandler<GetConnectionQuery, ConnectionDTO?>
{
    private readonly IConnectionQueryRepository _connectionQueryRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public GetConnectionHandler(IConnectionQueryRepository connectionQueryRepository, IUserQueryRepository userQueryRepository)
    {
        _connectionQueryRepository = connectionQueryRepository;
        _userQueryRepository = userQueryRepository;
    }

    public Task<ConnectionDTO?> Handle(GetConnectionQuery request, CancellationToken cancellationToken)
    {
        IUserAccount user1 = _userQueryRepository.GetUserById(request.UserId1)?.Account
            ?? throw new UserNotFoundException();
        IUserAccount user2 = _userQueryRepository.GetUserById(request.UserId2)?.Account
            ?? throw new UserNotFoundException();

        IConnection? connection = _connectionQueryRepository.GetConnection(user1, user2);

        return Task.FromResult(
            connection is not null
            ? new ConnectionDTO(connection.Account1.Id, connection.Account2.Id, connection.Status.ToString())
            : null);
    }
}
