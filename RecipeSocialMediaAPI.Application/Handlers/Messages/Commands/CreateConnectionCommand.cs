using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateConnectionCommand(NewConnectionContract Contract) : IRequest<ConnectionDto>;

internal class CreateConnectionHandler : IRequestHandler<CreateConnectionCommand, ConnectionDto>
{
    private readonly IConnectionPersistenceRepository _connectionPersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public CreateConnectionHandler(IConnectionPersistenceRepository connectionPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _connectionPersistenceRepository = connectionPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<ConnectionDto> Handle(CreateConnectionCommand request, CancellationToken cancellationToken)
    {
        IUserAccount user1 = (await _userQueryRepository
            .GetUserByIdAsync(request.Contract.UserId1, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.Contract.UserId1}");
        IUserAccount user2 = (await _userQueryRepository
            .GetUserByIdAsync(request.Contract.UserId2, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.Contract.UserId2}");

        IConnection createdConnection = await _connectionPersistenceRepository
            .CreateConnectionAsync(user1, user2, ConnectionStatus.Pending, cancellationToken);

        return new ConnectionDto(
            createdConnection.ConnectionId,
            createdConnection.Account1.Id, 
            createdConnection.Account2.Id, 
            createdConnection.Status.ToString());
    }
}
