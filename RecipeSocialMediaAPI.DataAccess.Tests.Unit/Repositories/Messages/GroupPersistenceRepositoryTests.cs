﻿using FluentAssertions;
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

public class GroupPersistenceRepositoryTests
{
    private readonly Mock<IMongoCollectionWrapper<GroupDocument>> _groupCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IGroupDocumentToModelMapper> _groupDocumentToModelMapperMock;

    private readonly GroupPersistenceRepository _groupPersistenceRepositorySUT;

    public GroupPersistenceRepositoryTests()
    {
        _groupCollectionMock = new Mock<IMongoCollectionWrapper<GroupDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<GroupDocument>())
            .Returns(_groupCollectionMock.Object);
        _groupDocumentToModelMapperMock = new Mock<IGroupDocumentToModelMapper>();

        _groupPersistenceRepositorySUT = new(_mongoCollectionFactoryMock.Object, _groupDocumentToModelMapperMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task CreateGroup_WhenGroupIsInsertedSuccessfully_ReturnsMappedGroupAsync()
    {
        // Given
        var groupName = "Group";
        var groupDesc = "Group Description";

        List<IUserAccount> users = new()
        {
            new TestUserAccount()
            {
                Id = "1",
                Handler = "user1",
                UserName = "User1",
                AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new TestUserAccount()
            {
                Id = "2",
                Handler = "user2",
                UserName = "User2",
                AccountCreationDate = new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
        };

        var userIds = users
            .Select(user => user.Id)
            .ToList();

        GroupDocument insertedDocument = new(
            Id: "g1",
            GroupName: groupName,
            GroupDescription: groupDesc,
            UserIds: userIds
        );

        _groupCollectionMock
            .Setup(collection => collection.Insert(
                It.Is<GroupDocument>(
                    groupDoc => groupDoc.Id == null
                         && groupDoc.GroupName == groupName
                         && groupDoc.GroupDescription == groupDesc
                         && groupDoc.UserIds.SequenceEqual(userIds)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(insertedDocument);

        Group expectedGroup = new(insertedDocument.Id!, insertedDocument.GroupName, insertedDocument.GroupDescription, users);

        _groupDocumentToModelMapperMock
            .Setup(mapper => mapper.MapGroupFromDocument(insertedDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGroup);

        // When
        var result = await _groupPersistenceRepositorySUT.CreateGroup(groupName, groupDesc, users);

        // Then
        result.Should().Be(expectedGroup);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateGroup_WhenUpdateIsSuccessful_ReturnTrueAsync()
    {
        // Given
        List<IUserAccount> users = new()
        {
            new TestUserAccount()
            {
                Id = "1",
                Handler = "user1",
                UserName = "User1",
                AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new TestUserAccount()
            {
                Id = "2",
                Handler = "user2",
                UserName = "User2",
                AccountCreationDate = new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
        };

        Group group = new("g1", "Group", "Group Description", users);

        Expression<Func<GroupDocument, bool>> expectedExpression = doc => doc.Id == group.GroupId;

        _groupCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.Is<GroupDocument>(
                    groupDoc => groupDoc.Id == group.GroupId
                             && groupDoc.GroupName == group.GroupName
                             && groupDoc.GroupDescription == group.GroupDescription
                             && groupDoc.UserIds.SequenceEqual(users.Select(user => user.Id))),
                It.Is<Expression<Func<GroupDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _groupPersistenceRepositorySUT.UpdateGroup(group);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateGroup_WhenUpdateIsUnsuccessful_ReturnFalseAsync()
    {
        // Given
        List<IUserAccount> users = new()
        {
            new TestUserAccount()
            {
                Id = "1",
                Handler = "user1",
                UserName = "User1",
                AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new TestUserAccount()
            {
                Id = "2",
                Handler = "user2",
                UserName = "User2",
                AccountCreationDate = new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
        };

        Group group = new("g1", "Group", "Group Description", users);

        Expression<Func<GroupDocument, bool>> expectedExpression = doc => doc.Id == group.GroupId;

        _groupCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.Is<GroupDocument>(
                    groupDoc => groupDoc.Id == group.GroupId
                             && groupDoc.GroupName == group.GroupName
                             && groupDoc.GroupDescription == group.GroupDescription
                             && groupDoc.UserIds.SequenceEqual(users.Select(user => user.Id))),
                It.Is<Expression<Func<GroupDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _groupPersistenceRepositorySUT.UpdateGroup(group);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task DeleteGroup_WhenDeleteIsSuccessful_ReturnTrueAsync()
    {
        // Given
        Group group = new("g1", "Group", "Group Description");

        _groupCollectionMock
            .Setup(collection => collection.Delete(
                It.IsAny<Expression<Func<GroupDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _groupPersistenceRepositorySUT.DeleteGroup(group);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task DeleteGroup_WhenDeleteIsUnsuccessful_ReturnFalseAsync()
    {
        // Given
        Group group = new("g1", "Group", "Group Description");

        _groupCollectionMock
            .Setup(collection => collection.Delete(
                It.IsAny<Expression<Func<GroupDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _groupPersistenceRepositorySUT.DeleteGroup(group);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task DeleteGroupById_WhenDeleteIsSuccessful_ReturnTrueAsync()
    {
        // Given
        _groupCollectionMock
            .Setup(collection => collection.Delete(
                It.IsAny<Expression<Func<GroupDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _groupPersistenceRepositorySUT.DeleteGroup("g1");

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task DeleteGroupById_WhenDeleteIsUnsuccessful_ReturnFalseAsync()
    {
        // Given
        _groupCollectionMock
            .Setup(collection => collection.Delete(
                It.IsAny<Expression<Func<GroupDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _groupPersistenceRepositorySUT.DeleteGroup("g1");

        // Then
        result.Should().BeFalse();
    }
}
