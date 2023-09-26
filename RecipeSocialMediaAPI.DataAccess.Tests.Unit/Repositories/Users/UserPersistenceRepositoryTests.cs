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
        UserDocument testDocument = new() { UserName = "TestName", Email = "TestEmail", Password = "TestPassword" };
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Insert(It.Is<UserDocument>(doc => doc == testDocument)))
            .Throws(new DocumentAlreadyExistsException<UserDocument>(testDocument));

        // When
        var action = () => _userPersistenceRepositorySUT.CreateUser(testDocument.UserName, testDocument.Email, testDocument.Password);

        // Then
        action.Should().Throw<DocumentAlreadyExistsException<UserDocument>>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void CreateUser_CreatesUserAndReturnsNewlyCreatedUser()
    {
        // Given
        UserDocument testDocument = new() { UserName = "TestName", Email = "TestEmail", Password = "TestPassword" };
        User testUser = new(testDocument.Id!, testDocument.UserName, testDocument.Email, testDocument.Password);
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Insert(It.Is<UserDocument>(doc => doc == testDocument)))
            .Returns(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapUserDocumentToUser(It.Is<UserDocument>(doc => doc == testDocument)))
            .Returns(testUser);

        // When
        var result = _userPersistenceRepositorySUT.CreateUser(testDocument.UserName, testDocument.Email, testDocument.Password);

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
        UserDocument testDocument = new() { UserName = "Initial Name", Email = "Initial Email", Password = "Initial Password" };
        User updatedUser = new(testDocument.Id!, "New Name", "New Email", "New Password");
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
                        && doc.UserName == updatedUser.UserName
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
        UserDocument testDocument = new() { UserName = "Initial Name", Email = "Initial Email", Password = "Initial Password" };
        User updatedUser = new(testDocument.Id!, "New Name", "New Email", "New Password");
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
        UserDocument testDocument = new() { UserName = "Initial Name", Email = "Initial Email", Password = "Initial Password" };
        User updatedUser = new(testDocument.Id!, "New Name", "New Email", "New Password");

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
        UserDocument testDocument = new() { UserName = "TestName", Email = "TestEmail", Password = "TestPassword" };
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
        UserDocument testDocument = new() { UserName = "TestName", Email = "TestEmail", Password = "TestPassword" };
        Expression<Func<UserDocument, bool>> expectedExpression = x => x.Id == testDocument.Id;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Delete(It.IsAny<Expression<Func<UserDocument, bool>>>()))
            .Returns(true);

        // When
        var result = _userPersistenceRepositorySUT.DeleteUser(new User(testDocument.Id!, testDocument.UserName, testDocument.Email, testDocument.Password));

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

        // When
        var result = _userPersistenceRepositorySUT.DeleteUser(new User("1", "Test Name", "Test Email", "Test Password"));

        // Then
        result.Should().BeFalse();
    }
}
