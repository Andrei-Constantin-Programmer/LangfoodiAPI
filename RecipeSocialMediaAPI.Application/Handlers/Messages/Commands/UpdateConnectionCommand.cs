using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateConnectionCommand(UpdateConnectionContract UpdateConnectionContract) : IRequest<bool>;

internal class UpdateConnectionHandler : IRequestHandler<UpdateConnectionCommand, bool>
{
    private readonly IConnectionQueryRepository _connectionQueryRepository;
    private readonly IConnectionPersistenceRepository _connectionPersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public UpdateConnectionHandler(IConnectionQueryRepository connectionQueryRepository, IConnectionPersistenceRepository connectionPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _connectionQueryRepository = connectionQueryRepository;
        _connectionPersistenceRepository = connectionPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public Task<bool> Handle(UpdateConnectionCommand request, CancellationToken cancellationToken)
    {
        IUserAccount user1 = _userQueryRepository.GetUserById(request.UpdateConnectionContract.UserId1)!.Account;
        IUserAccount user2 = _userQueryRepository.GetUserById(request.UpdateConnectionContract.UserId2)!.Account;

        IConnection connection = _connectionQueryRepository.GetConnection(user1, user2)!;

        Enum.TryParse(request.UpdateConnectionContract.NewConnectionStatus, out ConnectionStatus status);
        connection.Status = status;
            
        return Task.FromResult(_connectionPersistenceRepository.UpdateConnection(connection));
    }
}
