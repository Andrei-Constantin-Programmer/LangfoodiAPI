using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

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
        var user1 = _userQueryRepository
            .GetUserById(connectionDocument.AccountId1)?
            .Account
            ?? throw new UserDocumentNotFoundException(connectionDocument.AccountId1);

        var user2 = _userQueryRepository
            .GetUserById(connectionDocument.AccountId2)?
            .Account
            ?? throw new UserDocumentNotFoundException(connectionDocument.AccountId2);

        return new Connection(user1, user2, Enum.Parse<ConnectionStatus>(connectionDocument.ConnectionStatus));
    }
}
