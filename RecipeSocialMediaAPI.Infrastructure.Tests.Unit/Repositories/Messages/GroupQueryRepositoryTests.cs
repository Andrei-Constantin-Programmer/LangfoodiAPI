﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Repositories.Messages;

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
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetGroupById_WhenDocumentExists_ReturnMappedDocument()
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

        GroupDocument groupDoc = new(
            Id: "1",
            GroupName: "Group",
            GroupDescription: "Group Description",
            UserIds: userIds
        );

        Expression<Func<GroupDocument, bool>> expectedExpression = doc => doc.Id == groupDoc.Id;
        _groupCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<GroupDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupDoc);
        
        Group expectedGroup = new(groupDoc.Id!, groupDoc.GroupName, groupDoc.GroupDescription, users);

        _groupDocumentToModelMapperMock
            .Setup(mapper => mapper.MapGroupFromDocumentAsync(groupDoc, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGroup);

        // When
        var result = await _groupQueryRepositorySUT.GetGroupByIdAsync("1");

        // Then
        result.Should().Be(expectedGroup);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetGroupById_WhenDocumentIsNotInTheDatabase_ReturnNull()
    {
        // Given
        _groupCollectionMock
            .Setup(collection => collection.GetOneAsync(It.IsAny<Expression<Func<GroupDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupDocument?)null);

        // When
        var result = await _groupQueryRepositorySUT.GetGroupByIdAsync("1");

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetGroupById_WhenMongoThrowsException_LogExceptionAndReturnNull()
    {
        // Given
        Exception testException = new("Test Exception");
        _groupCollectionMock
            .Setup(collection => collection.GetOneAsync(It.IsAny<Expression<Func<GroupDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _groupQueryRepositorySUT.GetGroupByIdAsync("1");

        // Then
        result.Should().BeNull();
        _loggerMock
            .Verify(logger =>
                logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetGroupsByUser_WhenUserHasGroups_ReturnMappedGroups()
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
            new TestUserAccount()
            {
                Id = "u4",
                Handler = "user4",
                UserName = "User 4",
                AccountCreationDate = new(2024, 4, 4, 0, 0, 0, TimeSpan.Zero)
            },
            new TestUserAccount()
            {
                Id = "u5",
                Handler = "user5",
                UserName = "User 5",
                AccountCreationDate = new(2024, 5, 5, 0, 0, 0, TimeSpan.Zero)
            },
        };

        var userIds = users
            .Select(user => user.Id)
            .ToList();

        GroupDocument groupDoc1 = new(
            Id: "1",
            GroupName: "Group 1",
            GroupDescription: "Group Description 1",
            UserIds: userIds.Take(3).ToList()
        );

        GroupDocument groupDoc2 = new(
            Id: "2",
            GroupName: "Group 2",
            GroupDescription: "Group Description 2",
            UserIds: userIds.Take(1).Concat(userIds.Skip(3)).ToList()
        );

        _groupCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<GroupDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GroupDocument>() { groupDoc1, groupDoc2 });

        Group group1 = new(groupDoc1.Id!, groupDoc1.GroupName, groupDoc1.GroupDescription, users.Take(3));
        Group group2 = new(groupDoc2.Id!, groupDoc2.GroupName, groupDoc2.GroupDescription, users.Take(1).Concat(users.Skip(3)).ToList());

        _groupDocumentToModelMapperMock
            .Setup(mapper => mapper.MapGroupFromDocumentAsync(groupDoc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group1);
        _groupDocumentToModelMapperMock
            .Setup(mapper => mapper.MapGroupFromDocumentAsync(groupDoc2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group2);

        // When
        var result = await _groupQueryRepositorySUT.GetGroupsByUserAsync(users[0]);

        // Then
        result.Should().HaveCount(2);
        result.Should().Contain(group1);
        result.Should().Contain(group2);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetGroupsByUser_WhenGroupCantBeMapped_ReturnMappedGroupsIgnoringAndLoggingFailingGroup()
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
            new TestUserAccount()
            {
                Id = "u4",
                Handler = "user4",
                UserName = "User 4",
                AccountCreationDate = new(2024, 4, 4, 0, 0, 0, TimeSpan.Zero)
            },
            new TestUserAccount()
            {
                Id = "u5",
                Handler = "user5",
                UserName = "User 5",
                AccountCreationDate = new(2024, 5, 5, 0, 0, 0, TimeSpan.Zero)
            },
        };

        var userIds = users
            .Select(user => user.Id)
            .ToList();

        GroupDocument groupDoc1 = new(
            Id: "1",
            GroupName: "Group 1",
            GroupDescription: "Group Description 1",
            UserIds: userIds.Take(3).ToList()
        );

        GroupDocument groupDoc2 = new(
            Id: "2",
            GroupName: "Group 2",
            GroupDescription: "Group Description 2",
            UserIds: userIds.Take(1).Concat(userIds.Skip(3)).ToList()
        );

        _groupCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<GroupDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GroupDocument>() { groupDoc1, groupDoc2 });

        Group group1 = new(groupDoc1.Id!, groupDoc1.GroupName, groupDoc1.GroupDescription, users.Take(3));
        Group group2 = new(groupDoc2.Id!, groupDoc2.GroupName, groupDoc2.GroupDescription, users.Take(1).Concat(users.Skip(3)).ToList());

        Exception testException = new("Test Exception");
        _groupDocumentToModelMapperMock
            .Setup(mapper => mapper.MapGroupFromDocumentAsync(groupDoc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group1);
        _groupDocumentToModelMapperMock
            .Setup(mapper => mapper.MapGroupFromDocumentAsync(groupDoc2, It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _groupQueryRepositorySUT.GetGroupsByUserAsync(users[0]);

        // Then
        result.Should().HaveCount(1);
        result.Should().Contain(group1);
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }


    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetGroupsByUser_WhenUserHasNoGroups_ReturnEmptyCollection()
    {
        // Given
        TestUserAccount testUser = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1",
            AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        _groupCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<GroupDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GroupDocument>());

        // When
        var result = await _groupQueryRepositorySUT.GetGroupsByUserAsync(testUser);

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetGroupsByUser_WhenMongoThrowsAnException_LogErrorAndReturnEmptyCollection()
    {
        // Given
        TestUserAccount testUser = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1",
            AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        Exception testException = new("Test Exception");
        _groupCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<GroupDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _groupQueryRepositorySUT.GetGroupsByUserAsync(testUser);

        // Then
        result.Should().BeEmpty();
        _loggerMock
            .Verify(logger =>
                logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }
}
