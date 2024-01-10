using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class GroupQueryRepositoryTests
{
    private readonly Mock<ILogger<GroupQueryRepository>> _loggerMock;
    private readonly Mock<IGroupDocumentToModelMapper> _groupDocumentToModelMapperMock;
    private readonly Mock<IMongoCollectionWrapper<GroupDocument>> _groupCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;

    private readonly GroupQueryRepository _groupQueryRepositorySUT;

    public GroupQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<GroupQueryRepository>>();
        _groupDocumentToModelMapperMock = new Mock<IGroupDocumentToModelMapper>();
        _groupCollectionMock = new Mock<IMongoCollectionWrapper<GroupDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<GroupDocument>())
            .Returns(_groupCollectionMock.Object);

        _groupQueryRepositorySUT = new(_loggerMock.Object, _groupDocumentToModelMapperMock.Object, _mongoCollectionFactoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetGroupById_WhenDocumentExists_ReturnMappedDocument()
    {
        // Given
        List<IUserAccount> users = new()
        {
            new TestUserAccount()
            {
                Id = "u1",
                Handler = "user1",
                UserName = "User 1",
                AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new TestUserAccount()
            {
                Id = "u2",
                Handler = "user2",
                UserName = "User 2",
                AccountCreationDate = new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            new TestUserAccount()
            {
                Id = "u3",
                Handler = "user3",
                UserName = "User 3",
                AccountCreationDate = new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero)
            },
        };

        var userIds = users
            .Select(user => user.Id)
            .ToList();

        GroupDocument groupDoc = new()
        {
            Id = "1",
            GroupName = "Group",
            GroupDescription = "Group Description",
            UserIds = userIds
        };

        Expression<Func<GroupDocument, bool>> expectedExpression = doc => doc.Id == groupDoc.Id;
        _groupCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<GroupDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(groupDoc);
        
        Group expectedGroup = new(groupDoc.Id, groupDoc.GroupName, groupDoc.GroupDescription, users);

        _groupDocumentToModelMapperMock
            .Setup(mapper => mapper.MapGroupFromDocument(groupDoc))
            .Returns(expectedGroup);

        // When
        var result = _groupQueryRepositorySUT.GetGroupById("1");

        // Then
        result.Should().Be(expectedGroup);
    }
}
