using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers;

public class ConnectionDocumentToModelMapper : IConnectionDocumentToModelMapper
{
    private readonly IUserQueryRepository _userQueryRepository;

    public ConnectionDocumentToModelMapper(IUserQueryRepository userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
    }

    public async Task<IConnection> MapConnectionFromDocumentAsync(ConnectionDocument connectionDocument, CancellationToken cancellationToken = default)
    {
        IUserAccount user1 = (await _userQueryRepository.GetUserByIdAsync(connectionDocument.AccountId1, cancellationToken))?.Account
            ?? throw new UserDocumentNotFoundException(connectionDocument.AccountId1);

        IUserAccount user2 = (await _userQueryRepository.GetUserByIdAsync(connectionDocument.AccountId2, cancellationToken))?.Account
            ?? throw new UserDocumentNotFoundException(connectionDocument.AccountId2);

        return Enum.TryParse(connectionDocument.ConnectionStatus, out ConnectionStatus status)
            ? new Connection(connectionDocument.Id!, user1, user2, status)
            : throw new InvalidConnectionStatusException(connectionDocument);
    }
}
