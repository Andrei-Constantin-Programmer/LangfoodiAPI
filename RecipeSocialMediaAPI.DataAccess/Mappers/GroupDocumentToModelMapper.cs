using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public class GroupDocumentToModelMapper
{
    private readonly IUserQueryRepository _userQueryRepository;

    public GroupDocumentToModelMapper(IUserQueryRepository userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
        
    }

    public Group MapGroupFromDocument(GroupDocument groupDocument)
    {
        List<IUserAccount>  users = new List<IUserAccount>();
        foreach (string user in groupDocument.UserIds)
        {
            users.Add(_userQueryRepository
                .GetUserById(user)?.Account
                ?? throw new UserDocumentNotFoundException(user));
        }
        ImmutableList<IUserAccount> IUser = users.ToImmutableList();
        return new Group(groupDocument.GroupId, groupDocument.GroupName, groupDocument.GroupDescription, IUser);
    }
}
