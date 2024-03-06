using FluentAssertions;
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
    public async Task CreateUser_WhenDocumentAlreadyExistsExceptionIsThrownFromTheCollection_PropagateExceptionAsync()
    {
        // Given
        UserDocument testDocument = new( 
            Handler: "TestHandler", 
            UserName: "TestName", 
            Email: "TestEmail", 
            Password: "TestPassword", 
            AccountCreationDate: new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero),
            Role: (int)UserRole.User
        );

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Insert(It.Is<UserDocument>(doc => doc == testDocument), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DocumentAlreadyExistsException<UserDocument>(testDocument));

        // When
        var action = async () => await _userPersistenceRepositorySUT.CreateUser(testDocument.Handler, testDocument.UserName, testDocument.Email, testDocument.Password, testDocument.AccountCreationDate!.Value);

        // Then
        await action.Should().ThrowAsync<DocumentAlreadyExistsException<UserDocument>>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task CreateUser_CreatesUserAndReturnsNewlyCreatedUserAsync()
    {
        // Given
        UserDocument testDocument = new(
            Handler: "TestHandler",
            UserName: "TestName",
            Email: "TestEmail",
            Password: "TestPassword",
            AccountCreationDate: new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero),
            Role: (int)UserRole.User
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
            .Setup(collection => collection.Insert(It.Is<UserDocument>(doc => doc == testDocument), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(It.Is<UserDocument>(doc => doc == testDocument)))
            .Returns(testUser);

        // When
        var result = await _userPersistenceRepositorySUT.CreateUser(testDocument.Handler, testDocument.UserName, testDocument.Email, testDocument.Password, testDocument.AccountCreationDate!.Value);

        // Then
        result.Should().Be(testUser);
        _mongoCollectionWrapperMock
            .Verify(collection => collection.Insert(
                It.Is<UserDocument>(doc => doc == testDocument), 
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateUser_WhenUserExists_UpdatesUserAndReturnsTrueAsync()
    {
        // Given
        UserDocument testDocument = new(
            "Handler", 
            "Initial Name", 
            "Initial Email", 
            "Initial Password",
            (int)UserRole.User,
            "ProfileImageId", 
            new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero), 
            PinnedConversationIds: new List<string>() { "pid1", "pid2" }
        );

        IUserCredentials updatedUser = new TestUserCredentials
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!,
                Handler = "New Handler",
                UserName = "New Name",
                AccountCreationDate = testDocument.AccountCreationDate!.Value.AddDays(5),
                ProfileImageId = "NewImageId",
            },
            Email = "New Email",
            Password = "New Password"
        };

        updatedUser.Account.AddPin("pid1");
        updatedUser.Account.AddPin("pid2");

        Expression<Func<UserDocument, bool>> findExpression = x => x.Id == testDocument.Id;
        Expression<Func<UserDocument, bool>> updateExpression = x => x.Id == testDocument.Id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(
                It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, findExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);

        _mongoCollectionWrapperMock
            .Setup(collection => collection.UpdateRecord(
                It.IsAny<UserDocument>(), 
                It.IsAny<Expression<Func<UserDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _userPersistenceRepositorySUT.UpdateUser(updatedUser);

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
                        && doc.Password == updatedUser.Password
                        && doc.ProfileImageId == updatedUser.Account.ProfileImageId
                        && doc.PinnedConversationIds!.First() == updatedUser.Account.PinnedConversationIds[0]
                        && doc.PinnedConversationIds!.Skip(1).First() == updatedUser.Account.PinnedConversationIds[1]),
                    It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, updateExpression)), 
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateUser_WhenUserDoesNotExist_ReturnFalseAsync()
    {
        // Given
        UserDocument testDocument = new("Initial Handler", "Initial Name", "Initial Email", "Initial Password", (int)UserRole.User);

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
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<UserDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullUserDocument);

        // When
        var result = await _userPersistenceRepositorySUT.UpdateUser(updatedUser);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateUser_WhenCollectionCantUpdate_ReturnFalseAsync()
    {
        // Given
        UserDocument testDocument = new("Initial Handler", "Initial Name", "Initial Email", "Initial Password", (int)UserRole.User);
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
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<UserDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _mongoCollectionWrapperMock
            .Setup(collection => collection.UpdateRecord(
                It.IsAny<UserDocument>(), 
                It.IsAny<Expression<Func<UserDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _userPersistenceRepositorySUT.UpdateUser(updatedUser);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DeleteUser_WhenUserWithIdExists_DeleteUserAndReturnTrue()
    {
        // Given
        UserDocument testDocument = new("TestHandler", "TestName", "TestEmail", "TestPassword", (int)UserRole.User);
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
        UserDocument testDocument = new("TestHandler", "TestName", "TestEmail", "TestPassword", (int)UserRole.User);
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
