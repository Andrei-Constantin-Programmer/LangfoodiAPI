using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Services;
using System.Linq.Expressions;
using Neleus.LambdaCompare;

namespace RecipeSocialMediaAPI.Tests.Unit;

public class UserServiceTests
{
    internal Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    internal Mock<IMongoRepository<UserDocument>> _userRepositoryMock;
    internal UserService _userServiceSUT;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IMongoRepository<UserDocument>>();

        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.GetCollection<UserDocument>())
            .Returns(_userRepositoryMock.Object);

        _userServiceSUT = new UserService(_mongoCollectionFactoryMock.Object);
    }

    [Theory]
    [InlineData("testemail")]
    [InlineData("TESTEMAIL")]
    public void DoesEmailExist_WhenRepositoryContainsEmail_CheckEmailEqualityCaseInsensitiveAndReturnTrue(string emailToCheck)
    {
        // Given
        Expression<Func<UserDocument, bool>> testExpression = 
            user => user.Email.ToLower() == emailToCheck.ToLower();

        _userRepositoryMock
            .Setup(repo => repo.Contains(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, testExpression))))
            .Returns(true);
        _userRepositoryMock
            .Setup(repo => repo.Contains(It.Is<Expression<Func<UserDocument, bool>>>(expr => !Lambda.Eq(expr, testExpression))))
            .Returns(false);

        // When
        var result = _userServiceSUT.DoesEmailExist(emailToCheck);

        // Then
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("testemail")]
    [InlineData("TESTEMAIL")]
    public void DoesEmailExist_WhenRepositoryDoesNotContainsEmail_CheckEmailEqualityCaseInsensitiveAndReturnFalse(string emailToCheck)
    {
        // Given
        Expression<Func<UserDocument, bool>> testExpression =
            user => user.Email.ToLower() == emailToCheck.ToLower();

        _userRepositoryMock
            .Setup(repo => repo.Contains(It.Is<Expression<Func<UserDocument, bool>>>(expr => Lambda.Eq(expr, testExpression))))
            .Returns(false);

        // When
        var result = _userServiceSUT.DoesEmailExist(emailToCheck);

        // Then
        result.Should().BeFalse();
    }
}
