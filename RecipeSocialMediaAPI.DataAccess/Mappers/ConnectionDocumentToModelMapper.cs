using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

internal class ConnectionDocumentToModelMapper
{
    private readonly IUserQueryRepository _userQueryRepository;

    public ConnectionDocumentToModelMapper(IUserQueryRepository userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
    }

    public Connection MapConnectionFromDocument(ConnectionDocument connectionDocument)
    {
        IUserAccount user1 = _userQueryRepository
            .GetUserById(connectionDocument.AccountId1)?.Account
            ?? throw new UserDocumentNotFoundException(connectionDocument.AccountId1);

        IUserAccount user2 = _userQueryRepository
            .GetUserById(connectionDocument.AccountId2)?.Account
            ?? throw new UserDocumentNotFoundException(connectionDocument.AccountId2);

        return Enum.TryParse(connectionDocument.ConnectionStatus, out ConnectionStatus status)
            ? new Connection(user1, user2, status)
            : throw new InvalidConnectionStatusException(connectionDocument);
    }
}
