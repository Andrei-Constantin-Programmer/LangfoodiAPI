using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Tests.Shared.TestHelpers;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Users;

public class UserQueryRepositoryTests
{
    private readonly UserQueryRepository _userQueryRepositorySUT;
    private readonly Mock<IMongoCollectionWrapper<UserDocument>> _mongoCollectionWrapperMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IUserDocumentToModelMapper> _mapperMock;
    private readonly Mock<ILogger<UserQueryRepository>> _loggerMock;

    public UserQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<UserQueryRepository>>();
        _mapperMock = new Mock<IUserDocumentToModelMapper>();
        _mongoCollectionWrapperMock = new Mock<IMongoCollectionWrapper<UserDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<UserDocument>())
            .Returns(_mongoCollectionWrapperMock.Object);

        _userQueryRepositorySUT = new UserQueryRepository(_loggerMock.Object, _mapperMock.Object, _mongoCollectionFactoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetAllUsers_WhenNoUsersExist_ReturnsEmptyEnumerable()
    {
        // Given
        Expression<Func<UserDocument, bool>> expectedExpression = (_) => true;
        List<UserDocument> existingUsers = new();

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(existingUsers);

        // When
        var result = _userQueryRepositorySUT.GetAllUsers();

        // Then
        result.Should().BeEmpty();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.GetAll(It.IsAny<Expression<Func<UserDocument, bool>>>()), Times.Once);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void GetAllUsers_WhenUsersExist_ReturnsAllUsers(int numberOfUsers)
    {
        // Given
        Expression<Func<UserDocument, bool>> expectedExpression = (_) => true;
        List<UserDocument> existingUsers = new();
        for (int i = 0; i < numberOfUsers; i++)
        {
            existingUsers.Add(new UserDocument("TestHandler", "TestName", "TestEmail", "TestPassword"));
        }

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(existingUsers);

        // When
        var result = _userQueryRepositorySUT.GetAllUsers();

        // Then
        result.Should().HaveCount(numberOfUsers);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetUserById_WhenUserIsNotFound_ReturnNull()
    {
        // Given
        string id = "1";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == id;
        UserDocument? nullUserDocument = null;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(nullUserDocument);

        // When
        var result = _userQueryRepositorySUT.GetUserById(id);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetUserById_WhenUserIsFound_ReturnUser()
    {
        // Given
        string id = "1";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == id;
        UserDocument testDocument = new("TestHandler", "TestName", "TestEmail", "TestPassword");
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = testDocument.Handler,
                UserName = testDocument.UserName,
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };
            
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = _userQueryRepositorySUT.GetUserById(id);

        // Then
        result.Should().Be(testUser);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetUserById_WhenMongoThrowsException_LogExceptionAndReturnNull()
    {
        // Given
        string id = "1";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == id;
        UserDocument testDocument = new("TestHandler", "TestName", "TestEmail", "TestPassword");
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = testDocument.Handler,
                UserName = testDocument.UserName,
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };

        Exception testException = new("Test Exception");

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Throws(testException);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = _userQueryRepositorySUT.GetUserById(id);

        // Then
        result.Should().BeNull();
        _loggerMock
            .Verify(logger => 
                logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetUserByEmail_WhenUserIsNotFound_ReturnNull()
    {
        // Given
        string email = "test@mail.com";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Email == email;
        UserDocument? nullUserDocument = null;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(nullUserDocument);

        // When
        var result = _userQueryRepositorySUT.GetUserByEmail(email);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetUserByEmail_WhenUserIsFound_ReturnUser()
    {
        // Given
        string email = "test@mail.com";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Email == email;
        UserDocument testDocument = new("TestHandler", "TestName", "TestEmail", "TestPassword");
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = testDocument.Handler,
                UserName = testDocument.UserName,
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };


        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = _userQueryRepositorySUT.GetUserByEmail(email);

        // Then
        result.Should().Be(testUser);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetUserByEmail_WhenMongoThrowsException_LogExceptionAndReturnNull()
    {
        // Given
        string email = "test@mail.com";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Email == email;
        UserDocument testDocument = new("TestHandler", "TestName", email, "TestPassword");
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = testDocument.Handler,
                UserName = testDocument.UserName,
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };

        Exception testException = new("Test Exception");

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Throws(testException);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = _userQueryRepositorySUT.GetUserByEmail(email);

        // Then
        result.Should().BeNull();
        _loggerMock
            .Verify(logger =>
                logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetUserByUsername_WhenUserIsNotFound_ReturnNull()
    {
        // Given
        string username = "TestUsername";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.UserName == username;
        UserDocument? nullUserDocument = null;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(nullUserDocument);

        // When
        var result = _userQueryRepositorySUT.GetUserByUsername(username);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetUserByUsername_WhenUserIsFound_ReturnUser()
    {
        // Given
        string username = "WrongUsername";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.UserName == username;
        UserDocument testDocument = new("TestHandler", "TestName", "TestEmail", "TestPassword");
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = testDocument.Handler,
                UserName = testDocument.UserName,
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = _userQueryRepositorySUT.GetUserByUsername(username);

        // Then
        result.Should().Be(testUser);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetUserByUsername_WhenMongoThrowsException_LogExceptionAndReturnNull()
    {
        // Given
        string username = "TestUsername";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == username;
        UserDocument testDocument = new("TestHandler", "TestName", "TestEmail", "TestPassword");
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = testDocument.Handler,
                UserName = testDocument.UserName,
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };

        Exception testException = new("Test Exception");

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Throws(testException);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = _userQueryRepositorySUT.GetUserById(username);

        // Then
        result.Should().BeNull();
        _loggerMock
            .Verify(logger =>
                logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetAllUserAccountsContaining_WhenThereAreNoMatchingUsers_ReturnEmptyEnumerable()
    {
        // Given
        string containedString = "test";
        Expression<Func<UserDocument, bool>> expectedExpression = x
            => x.Handler.Contains(containedString, StringComparison.InvariantCultureIgnoreCase)
            && x.UserName.Contains(containedString, StringComparison.InvariantCultureIgnoreCase);

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(Enumerable.Empty<UserDocument>());

        // When
        var result = _userQueryRepositorySUT.GetAllUserAccountsContaining(containedString);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetAllUserAccountsContaining_WhenThereAreMatchingUsers_ReturnUsers()
    {
        // Given
        string containedString = "test";
        Expression<Func<UserDocument, bool>> expectedExpression = x
            => x.Handler.Contains(containedString, StringComparison.InvariantCultureIgnoreCase)
            && x.UserName.Contains(containedString, StringComparison.InvariantCultureIgnoreCase);

        List<UserDocument> userDocuments = new()
        {
            new("non_searched_handle", $"User{containedString}Name", string.Empty, string.Empty),
            new($"the_{containedString}_handle", "NonSearchedUsername", string.Empty, string.Empty),
        };

        IUserCredentials testUser1 = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = userDocuments[0].Handler,
                UserName = userDocuments[0].UserName,
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "email",
            Password = "password"
        };
        IUserCredentials testUser2 = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = userDocuments[1].Handler,
                UserName = userDocuments[1].UserName,
                AccountCreationDate = new(2023, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "email",
            Password = "password"
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(userDocuments);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(userDocuments[0]))
            .Returns(testUser1);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(userDocuments[1]))
            .Returns(testUser2);

        // When
        var result = _userQueryRepositorySUT.GetAllUserAccountsContaining(containedString);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new List<IUserAccount> { testUser1.Account, testUser2.Account });
    }
}
