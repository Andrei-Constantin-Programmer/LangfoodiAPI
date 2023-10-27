using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

internal class ConnectionDocumentToModelMapper
{
    private readonly IUserQueryRepository _userQueryRepository;

    public ConnectionDocumentToModelMapper(IUserQueryRepository userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
    }
}
