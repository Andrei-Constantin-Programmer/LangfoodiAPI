﻿using FluentAssertions;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Users;

public class UserPersistenceRepositoryTests
{
    private readonly UserPersistenceRepository _userPersistenceRepositorySUT;
    private readonly Mock<IMongoCollectionWrapper<UserDocument>> _mongoCollectionWrapperMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IUserDocumentToModelMapper> _mapperMock;

    public UserPersistenceRepositoryTests()
    {
        _mapperMock = new Mock<IUserDocumentToModelMapper>();
        _mongoCollectionWrapperMock = new Mock<IMongoCollectionWrapper<UserDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<UserDocument>())
            .Returns(_mongoCollectionWrapperMock.Object);

        _userPersistenceRepositorySUT = new UserPersistenceRepository(_mapperMock.Object, _mongoCollectionFactoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void CreateUser_WhenDocumentAlreadyExistsExceptionIsThrownFromTheCollection_PropagateException()
    {
        // Given
        UserDocument testDocument = new( 
            Handler: "TestHandler", 
            UserName: "TestName", 
            Email: "TestEmail", 
            Password: "TestPassword", 
            AccountCreationDate: new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero) 
        );

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Insert(It.Is<UserDocument>(doc => doc == testDocument)))
            .Throws(new DocumentAlreadyExistsException<UserDocument>(testDocument));

        // When
        var action = () => _userPersistenceRepositorySUT.CreateUser(testDocument.Handler, testDocument.UserName, testDocument.Email, testDocument.Password, testDocument.AccountCreationDate!.Value);

        // Then
        action.Should().Throw<DocumentAlreadyExistsException<UserDocument>>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void CreateUser_CreatesUserAndReturnsNewlyCreatedUser()
    {
        // Given
        UserDocument testDocument = new(
            Handler: "TestHandler",
            UserName: "TestName",
            Email: "TestEmail",
            Password: "TestPassword",
            AccountCreationDate: new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        );

        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!, 
                Handler = testDocument.Handler, 
                UserName = testDocument.UserName, 
                AccountCreationDate = testDocument.AccountCreationDate!.Value
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Insert(It.Is<UserDocument>(doc => doc == testDocument)))
            .Returns(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(It.Is<UserDocument>(doc => doc == testDocument)))
            .Returns(testUser);

        // When
        var result = _userPersistenceRepositorySUT.CreateUser(testDocument.Handler, testDocument.UserName, testDocument.Email, testDocument.Password, testDocument.AccountCreationDate!.Value);

        // Then
        result.Should().Be(testUser);
        _mongoCollectionWrapperMock
            .Verify(collection => collection.Insert(It.Is<UserDocument>(doc => doc == testDocument)), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateUser_WhenUserExists_UpdatesUserAndReturnsTrue()
    {
        // Given
        UserDocument testDocument = new("Handler", "Initial Name", "Initial Email", "Initial Password", new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero));

        IUserCredentials updatedUser = new TestUserCredentials
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!,
                Handler = "New Handler",
                UserName = "New Name",
                AccountCreationDate = testDocument.AccountCreationDate!.Value.AddDays(5)
            },
            Email = "New Email",
            Password = "New Password"
        };
        Expression<Func<UserDocument, bool>> findExpression = x => x.Id == testDocument.Id;
        Expression<Func<UserDocument, bool>> updateExpression = x => x.Id == testDocument.Id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, findExpression))))
            .Returns(testDocument);

        _mongoCollectionWrapperMock
            .Setup(collection => collection.UpdateRecord(It.IsAny<UserDocument>(), It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Returns(true);

        // When
        var result = _userPersistenceRepositorySUT.UpdateUser(updatedUser);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.UpdateRecord(
                    It.Is<UserDocument>(
                        doc => doc.Id == testDocument.Id
                        && doc.Handler == testDocument.Handler
                        && doc.AccountCreationDate == testDocument.AccountCreationDate
                        && doc.UserName == updatedUser.Account.UserName
                        && doc.Email == updatedUser.Email
                        && doc.Password == updatedUser.Password),
                    It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, updateExpression))),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateUser_WhenUserDoesNotExist_ReturnFalse()
    {
        // Given
        UserDocument testDocument = new("Initial Handler", "Initial Name", "Initial Email", "Initial Password");

        IUserCredentials updatedUser = new TestUserCredentials
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!,
                Handler = "New Handler",
                UserName = "New Name",
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "New Email",
            Password = "New Password"
        };
        UserDocument? nullUserDocument = null;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Returns(nullUserDocument);

        // When
        var result = _userPersistenceRepositorySUT.UpdateUser(updatedUser);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateUser_WhenCollectionCantUpdate_ReturnFalse()
    {
        // Given
        UserDocument testDocument = new("Initial Handler", "Initial Name", "Initial Email", "Initial Password");
        IUserCredentials updatedUser = new TestUserCredentials
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!,
                Handler = "New Handler",
                UserName = "New Name",
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "New Email",
            Password = "New Password"
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Returns(testDocument);
        _mongoCollectionWrapperMock
            .Setup(collection => collection.UpdateRecord(It.IsAny<UserDocument>(), It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Returns(false);

        // When
        var result = _userPersistenceRepositorySUT.UpdateUser(updatedUser);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DeleteUser_WhenUserWithIdExists_DeleteUserAndReturnTrue()
    {
        // Given
        UserDocument testDocument = new("TestHandler", "TestName", "TestEmail", "TestPassword");
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == testDocument.Id;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Delete(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Returns(true);

        // When
        var result = _userPersistenceRepositorySUT.DeleteUser(testDocument.Id!);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock.Verify(collection =>
            collection.Delete(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DeleteUser_WhenUserExists_DeleteUserAndReturnTrue()
    {
        // Given
        UserDocument testDocument = new("TestHandler", "TestName", "TestEmail", "TestPassword");
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == testDocument.Id;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Delete(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Returns(true);

        IUserCredentials userCredentials = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!,
                Handler = testDocument.Handler,
                UserName = testDocument.UserName,
                AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };

        // When
        var result = _userPersistenceRepositorySUT.DeleteUser(userCredentials);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock.Verify(collection =>
            collection.Delete(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DeleteUser_WhenUserWithIdDoesNotExist_ReturnFalse()
    {
        // Given
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Delete(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Returns(false);

        // When
        var result = _userPersistenceRepositorySUT.DeleteUser("1");

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DeleteUser_WhenUserDoesNotExist_ReturnFalse()
    {
        // Given
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Delete(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Returns(false);

        IUserCredentials userCredentials = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "1",
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "Test Email",
            Password = "Test Password"
        };

        // When
        var result = _userPersistenceRepositorySUT.DeleteUser(userCredentials);

        // Then
        result.Should().BeFalse();
    }
}
