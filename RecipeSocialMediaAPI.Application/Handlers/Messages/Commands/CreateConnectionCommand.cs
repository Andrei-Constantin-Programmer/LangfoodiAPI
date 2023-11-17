using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateConnectionCommand(NewConnectionContract NewConnectionContract) : IValidatableRequest<ConnectionDTO>;

internal class CreateConnectionHandler : IRequestHandler<CreateConnectionCommand, ConnectionDTO>
{
    private readonly IConnectionPersistenceRepository _connectionPersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public CreateConnectionHandler(IConnectionPersistenceRepository connectionPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _connectionPersistenceRepository = connectionPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<ConnectionDTO> Handle(CreateConnectionCommand request, CancellationToken cancellationToken)
    {
        IUserAccount user1 = _userQueryRepository
            .GetUserById(request.NewConnectionContract.UserId1)?.Account
            ?? throw new UserNotFoundException();
        IUserAccount user2 = _userQueryRepository
            .GetUserById(request.NewConnectionContract.UserId2)?.Account
            ?? throw new UserNotFoundException();

        IConnection createdConnection = _connectionPersistenceRepository
            .CreateConnection(user1, user2, ConnectionStatus.Pending);

        return await Task.FromResult(new ConnectionDTO(
            createdConnection.Account1.Id, 
            createdConnection.Account2.Id, 
            createdConnection.Status.ToString()));
    }
}
