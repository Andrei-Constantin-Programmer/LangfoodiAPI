using FluentAssertions;
using MongoDB.Driver;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.Tests.Integration.IntegrationHelpers;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Integration.MongoConfiguration;

public class MongoCollectionWrapperTests : IClassFixture<MongoDBFixture>
{
    private readonly MongoDBFixture _dbFixture;
    private readonly MongoCollectionWrapper<TestDocument> _mongoCollectionWrapperSUT;

    public MongoCollectionWrapperTests(MongoDBFixture dbFixture)
    {
        _dbFixture = dbFixture;
        _dbFixture.CleanupCollection();

        DatabaseConfiguration testConfiguration = new(dbFixture.ConnectionString, dbFixture.DatabaseName);
        _mongoCollectionWrapperSUT = new MongoCollectionWrapper<TestDocument>(testConfiguration);
    }

    [Fact]
    public void Insert_AddsDocumentToTheDatabaseAndReturnsDocument()
    {
        // Given
        TestDocument testDocument = new() { Id = null, TestProperty = "Test 1" };

        // When
        var document = _mongoCollectionWrapperSUT.Insert(testDocument);

        // Then
        document.Should().Be(testDocument);
        var documentFromDb = _dbFixture.TestCollection.Find(doc => doc.Id == testDocument.Id).First();
        documentFromDb.Should().Be(testDocument);
    }

    [Fact]
    public void Delete_WhenDocumentWithConditionExists_DeleteDocumentAndReturnTrue()
    {
        // Given
        TestDocument testDocument = new() { Id = null, TestProperty = "Test 1" };
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var wasDeleted = _mongoCollectionWrapperSUT.Delete(doc => doc.Id == testDocument.Id);

        // Then
        wasDeleted.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().BeEmpty();
    }

    [Fact]
    public void Delete_WhenMultipleDocumentsWithConditionExist_DeleteFirstFittingDocumentAndReturnTrue()
    {
        // Given
        List<TestDocument> testDocuments = new()
        {
            new() { Id = null, TestProperty = "To Delete 1" },
            new() { Id = null, TestProperty = "To Delete 2" },
            new() { Id = null, TestProperty = "To Keep 1" },
            new() { Id = null, TestProperty = "To Keep 2" },
        };
        _dbFixture.TestCollection.InsertMany(testDocuments);

        // When
        var wasDeleted = _mongoCollectionWrapperSUT.Delete(doc => doc.TestProperty!.Contains("To Delete"));

        // Then
        wasDeleted.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList()
            .Should().NotContain(doc => doc.TestProperty == "To Delete 1")
            .And.HaveCount(3);
    }

    [Fact]
    public void Delete_WhenNoDocumentWithConditionExists_ReturnFalse()
    {
        // Given
        TestDocument testDocument = new() { Id = null, TestProperty = "Test 1" };
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var wasDeleted = _mongoCollectionWrapperSUT.Delete(doc => doc.TestProperty == string.Empty);

        // Then
        wasDeleted.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().Contain(doc => doc == testDocument);
    }


}
