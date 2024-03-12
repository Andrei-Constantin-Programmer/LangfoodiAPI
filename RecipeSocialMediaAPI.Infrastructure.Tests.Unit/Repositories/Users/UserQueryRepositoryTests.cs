using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Infrastructure.Repositories.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Repositories.Users;

public class UserQueryRepositoryTests
{
    private readonly Mock<IMongoCollectionWrapper<UserDocument>> _mongoCollectionWrapperMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IUserDocumentToModelMapper> _mapperMock;
    private readonly Mock<ILogger<UserQueryRepository>> _loggerMock;
    private readonly IDataCryptoService _dataCryptoServiceFake;

    private readonly UserQueryRepository _userQueryRepositorySUT;

    public UserQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<UserQueryRepository>>();
        _mapperMock = new Mock<IUserDocumentToModelMapper>();
        _mongoCollectionWrapperMock = new Mock<IMongoCollectionWrapper<UserDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<UserDocument>())
            .Returns(_mongoCollectionWrapperMock.Object);
        _dataCryptoServiceFake = new FakeDataCryptoService();

        _userQueryRepositorySUT = new UserQueryRepository(
            _loggerMock.Object,
            _mapperMock.Object,
            _mongoCollectionFactoryMock.Object,
            _dataCryptoServiceFake);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetUserById_WhenUserIsNotFound_ReturnNull()
    {
        // Given
        string id = "1";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == id;
        UserDocument? nullUserDocument = null;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullUserDocument);

        // When
        var result = await _userQueryRepositorySUT.GetUserByIdAsync(id);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetUserById_WhenUserIsFound_ReturnUser()
    {
        // Given
        string id = "1";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == id;
        UserDocument testDocument = new(
            _dataCryptoServiceFake.Encrypt("TestHandler"),
            _dataCryptoServiceFake.Encrypt("TestName"),
            _dataCryptoServiceFake.Encrypt("TestEmail"),
            _dataCryptoServiceFake.Encrypt("TestPassword"),
            (int)UserRole.User);
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                UserName = _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = _dataCryptoServiceFake.Decrypt(testDocument.Email),
            Password = _dataCryptoServiceFake.Decrypt(testDocument.Password)
        };
            
        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = await _userQueryRepositorySUT.GetUserByIdAsync(id);

        // Then
        result.Should().Be(testUser);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetUserById_WhenMongoThrowsException_LogExceptionAndReturnNull()
    {
        // Given
        string id = "1";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == id;
        UserDocument testDocument = new(
            _dataCryptoServiceFake.Encrypt("TestHandler"),
            _dataCryptoServiceFake.Encrypt("TestName"),
            _dataCryptoServiceFake.Encrypt("TestEmail"),
            _dataCryptoServiceFake.Encrypt("TestPassword"),
            (int)UserRole.User);
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                UserName = _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = _dataCryptoServiceFake.Decrypt(testDocument.Email),
            Password = _dataCryptoServiceFake.Decrypt(testDocument.Password)
        };

        Exception testException = new("Test Exception");

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetOneAsync(It.IsAny<Expression<Func<UserDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = await _userQueryRepositorySUT.GetUserByIdAsync(id);

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
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetUserByEmail_WhenUserIsNotFound_ReturnNull()
    {
        // Given
        string email = "test@mail.com";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Email == email;
        UserDocument? nullUserDocument = null;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullUserDocument);

        // When
        var result = await _userQueryRepositorySUT.GetUserByEmailAsync(email);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetUserByEmail_WhenUserIsFound_ReturnUser()
    {
        // Given
        string email = "test@mail.com";
        Expression<Func<UserDocument, bool>> expectedExpression = x => _dataCryptoServiceFake.Decrypt(x.Email) == email;
        UserDocument testDocument = new(
            _dataCryptoServiceFake.Encrypt("TestHandler"),
            _dataCryptoServiceFake.Encrypt("TestName"),
            _dataCryptoServiceFake.Encrypt("TestEmail"),
            _dataCryptoServiceFake.Encrypt("TestPassword"),
            (int)UserRole.User);
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                UserName = _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = _dataCryptoServiceFake.Decrypt(testDocument.Email),
            Password = _dataCryptoServiceFake.Decrypt(testDocument.Password)
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = await _userQueryRepositorySUT.GetUserByEmailAsync(email);

        // Then
        result.Should().Be(testUser);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetUserByEmail_WhenMongoThrowsException_LogExceptionAndReturnNull()
    {
        // Given
        string email = "test@mail.com";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Email == email;
        UserDocument testDocument = new(
            _dataCryptoServiceFake.Encrypt("TestHandler"),
            _dataCryptoServiceFake.Encrypt("TestName"),
            _dataCryptoServiceFake.Encrypt(email),
            _dataCryptoServiceFake.Encrypt("TestPassword"),
            (int)UserRole.User);
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                UserName = _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = _dataCryptoServiceFake.Decrypt(testDocument.Email),
            Password = _dataCryptoServiceFake.Decrypt(testDocument.Password)
        };

        Exception testException = new("Test Exception");

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetOneAsync(It.IsAny<Expression<Func<UserDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = await _userQueryRepositorySUT.GetUserByEmailAsync(email);

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
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetUserByUsername_WhenUserIsNotFound_ReturnNull()
    {
        // Given
        string username = "TestUsername";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.UserName == username;
        UserDocument? nullUserDocument = null;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullUserDocument);

        // When
        var result = await _userQueryRepositorySUT.GetUserByUsernameAsync(username);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetUserByUsername_WhenUserIsFound_ReturnUser()
    {
        // Given
        string username = "WrongUsername";
        Expression<Func<UserDocument, bool>> expectedExpression = x => _dataCryptoServiceFake.Decrypt(x.UserName) == username;
        UserDocument testDocument = new(
            _dataCryptoServiceFake.Encrypt("TestHandler"),
            _dataCryptoServiceFake.Encrypt("TestName"),
            _dataCryptoServiceFake.Encrypt("TestEmail"),
            _dataCryptoServiceFake.Encrypt("TestPassword"),
            (int)UserRole.User);
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                UserName = _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = _dataCryptoServiceFake.Decrypt(testDocument.Email),
            Password = _dataCryptoServiceFake.Decrypt(testDocument.Password)
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = await _userQueryRepositorySUT.GetUserByUsernameAsync(username);

        // Then
        result.Should().Be(testUser);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetUserByUsername_WhenMongoThrowsException_LogExceptionAndReturnNull()
    {
        // Given
        string username = "TestUsername";
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == username;
        UserDocument testDocument = new(
            _dataCryptoServiceFake.Encrypt("TestHandler"),
            _dataCryptoServiceFake.Encrypt("TestName"),
            _dataCryptoServiceFake.Encrypt("TestEmail"),
            _dataCryptoServiceFake.Encrypt("TestPassword"),
            (int)UserRole.User);
        IUserCredentials testUser = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                UserName = _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            Email = _dataCryptoServiceFake.Decrypt(testDocument.Email),
            Password = _dataCryptoServiceFake.Decrypt(testDocument.Password)
        };

        Exception testException = new("Test Exception");

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetOneAsync(It.IsAny<Expression<Func<UserDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(testDocument))
            .Returns(testUser);

        // When
        var result = await _userQueryRepositorySUT.GetUserByIdAsync(username);

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
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetAllUserAccountsContaining_WhenThereAreNoMatchingUsers_ReturnEmptyEnumerable()
    {
        // Given
        string containedString = "test";
        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<UserDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<UserDocument>());

        // When
        var result = await _userQueryRepositorySUT.GetAllUserAccountsContainingAsync(containedString);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetAllUserAccountsContaining_WhenThereAreMatchingUsers_ReturnUsers()
    {
        // Given
        string containedString = "test";
        Expression<Func<UserDocument, bool>> expectedExpression = x
            => _dataCryptoServiceFake.Decrypt(x.Handler).Contains(containedString.ToLower())
            || _dataCryptoServiceFake.Decrypt(x.UserName).Contains(containedString.ToLower());

        List<UserDocument> userDocuments = new()
        {
            new(
                _dataCryptoServiceFake.Encrypt("non_searched_handle"),
                _dataCryptoServiceFake.Encrypt($"User{containedString}Name"),
                _dataCryptoServiceFake.Encrypt(string.Empty),
                _dataCryptoServiceFake.Encrypt(string.Empty),
                (int)UserRole.User),
            new(
                _dataCryptoServiceFake.Encrypt($"the_{containedString}_handle"),
                _dataCryptoServiceFake.Encrypt("NonSearchedUsername"),
                _dataCryptoServiceFake.Encrypt(string.Empty),
                _dataCryptoServiceFake.Encrypt(string.Empty),
                (int)UserRole.User),
        };

        IUserCredentials testUser1 = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = _dataCryptoServiceFake.Decrypt(userDocuments[0].Handler),
                UserName = _dataCryptoServiceFake.Decrypt(userDocuments[0].UserName),
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
                Handler = _dataCryptoServiceFake.Decrypt(userDocuments[1].Handler),
                UserName = _dataCryptoServiceFake.Decrypt(userDocuments[1].UserName),
                AccountCreationDate = new(2023, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "email",
            Password = "password"
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAllAsync(
                It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDocuments);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(userDocuments[0]))
            .Returns(testUser1);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(userDocuments[1]))
            .Returns(testUser2);

        // When
        var result = await _userQueryRepositorySUT.GetAllUserAccountsContainingAsync(containedString);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new List<IUserAccount> { testUser1.Account, testUser2.Account });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetAllUserAccountsContaining_WhenUserMappingFails_LogAndIgnoreUser()
    {
        // Given
        string containedString = "test";
        Expression<Func<UserDocument, bool>> expectedExpression = x
            => _dataCryptoServiceFake.Decrypt(x.Handler).Contains(containedString.ToLower())
            || _dataCryptoServiceFake.Decrypt(x.UserName).Contains(containedString.ToLower());

        List<UserDocument> userDocuments = new()
        {
            new(
                _dataCryptoServiceFake.Encrypt("non_searched_handle"),
                _dataCryptoServiceFake.Encrypt($"User{containedString}Name"),
                _dataCryptoServiceFake.Encrypt(string.Empty),
                _dataCryptoServiceFake.Encrypt(string.Empty),
                (int)UserRole.User),
            new(
                _dataCryptoServiceFake.Encrypt($"the_{containedString}_handle"),
                _dataCryptoServiceFake.Encrypt("NonSearchedUsername"),
                _dataCryptoServiceFake.Encrypt(string.Empty),
                _dataCryptoServiceFake.Encrypt(string.Empty),
                (int)UserRole.User),
        };

        IUserCredentials testUser1 = new TestUserCredentials()
        {
            Account = new TestUserAccount()
            {
                Id = "TestId",
                Handler = _dataCryptoServiceFake.Decrypt(userDocuments[0].Handler),
                UserName = _dataCryptoServiceFake.Decrypt(userDocuments[0].UserName),
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
                Handler = _dataCryptoServiceFake.Decrypt(userDocuments[1].Handler),
                UserName = _dataCryptoServiceFake.Decrypt(userDocuments[1].UserName),
                AccountCreationDate = new(2023, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "email",
            Password = "password"
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAllAsync(
                It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDocuments);

        Exception testException = new("Test Exception");
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(userDocuments[0]))
            .Returns(testUser1);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(userDocuments[1]))
            .Throws(testException);

        // When
        var result = await _userQueryRepositorySUT.GetAllUserAccountsContainingAsync(containedString);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new List<IUserAccount> { testUser1.Account });
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
