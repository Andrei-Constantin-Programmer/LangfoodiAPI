using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public class GroupDocumentToModelMapper : IGroupDocumentToModelMapper
{
    private readonly IUserQueryRepository _userQueryRepository;

    public GroupDocumentToModelMapper(IUserQueryRepository userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;  
    }

    public Group MapGroupFromDocument(GroupDocument groupDocument)
    {
        List<IUserAccount> users = new();
        foreach (string user in groupDocument.UserIds)
        {
            users.Add(_userQueryRepository
                .GetUserById(user)?.Account
                ?? throw new UserDocumentNotFoundException(user));
        }

        return new Group(groupDocument.Id!, groupDocument.GroupName, groupDocument.GroupDescription, users.ToImmutableList());
    }
}
