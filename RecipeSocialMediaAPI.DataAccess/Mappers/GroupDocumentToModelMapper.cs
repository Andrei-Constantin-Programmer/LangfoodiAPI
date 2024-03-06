using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public class GroupDocumentToModelMapper : IGroupDocumentToModelMapper
{
    private readonly IUserQueryRepository _userQueryRepository;

    public GroupDocumentToModelMapper(IUserQueryRepository userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;  
    }

    public async Task<Group> MapGroupFromDocument(GroupDocument groupDocument, CancellationToken cancellationToken = default)
    {
        if (groupDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Group Document with null ID to Group");
        }

        var users = await Task.WhenAll(groupDocument.UserIds
            .Select(async userId => (await _userQueryRepository.GetUserById(userId, cancellationToken))?.Account
                                    ?? throw new UserDocumentNotFoundException(userId)));
        
        return new Group(groupDocument.Id, groupDocument.GroupName, groupDocument.GroupDescription, users);
    }
}
