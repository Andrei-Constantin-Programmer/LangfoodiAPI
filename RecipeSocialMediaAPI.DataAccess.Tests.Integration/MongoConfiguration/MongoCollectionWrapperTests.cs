using FluentAssertions;
using MongoDB.Driver;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.DataAccess.Tests.Shared.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure.Traits;

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
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetAll_WhenThereAreDocuments_ReturnAllDocumentsWithProperty()
    {
        // Given
        List<TestDocument> existingDocuments = new();
        for(int i = 0; i <= 10; i++)
        {
            existingDocuments.Add(new() { TestProperty = i.ToString() });
        }

        _dbFixture.TestCollection.InsertMany(existingDocuments);

        // When
        var result = _mongoCollectionWrapperSUT.GetAll(doc => doc.TestProperty!.StartsWith('1'));

        // Then
        result.Should().OnlyContain(doc => doc.TestProperty == "1" || doc.TestProperty == "10");
        result.Should().HaveCount(2);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetAll_WhenNoDocumentsMatch_ReturnEmptyList()
    {
        // Given
        List<TestDocument> existingDocuments = new();
        for (int i = 0; i <= 10; i++)
        {
            existingDocuments.Add(new() { TestProperty = i.ToString() });
        }

        _dbFixture.TestCollection.InsertMany(existingDocuments);

        // When
        var result = _mongoCollectionWrapperSUT.GetAll(doc => doc.TestProperty!.StartsWith('a'));

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetAll_WhenThereAreNoDocuments_ReturnEmptyList()
    {
        // Given

        // When
        var result = _mongoCollectionWrapperSUT.GetAll(_ => true);

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Insert_AddsDocumentToTheDatabaseAndReturnsDocument()
    {
        // Given
        TestDocument testDocument = new() { TestProperty = "Test 1" };

        // When
        var document = _mongoCollectionWrapperSUT.Insert(testDocument);

        // Then
        document.Should().Be(testDocument);
        var documentFromDb = _dbFixture.TestCollection.Find(doc => doc.Id == testDocument.Id).First();
        documentFromDb.Should().Be(testDocument);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Insert_WhenDocumentWithIdAlreadyExists_ThrowDocumentAlreadyExistsException()
    {
        // Given
        TestDocument testDocument = new() { TestProperty = "Test 1" };
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var action = () => _mongoCollectionWrapperSUT.Insert(testDocument);

        // Then
        action.Should().Throw<DocumentAlreadyExistsException<TestDocument>>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Delete_WhenDocumentWithConditionExists_DeleteDocumentAndReturnTrue()
    {
        // Given
        TestDocument testDocument = new() { TestProperty = "Test 1" };
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var wasDeleted = _mongoCollectionWrapperSUT.Delete(doc => doc.Id == testDocument.Id);

        // Then
        wasDeleted.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Delete_WhenMultipleDocumentsWithConditionExist_DeleteFirstFittingDocumentAndReturnTrue()
    {
        // Given
        List<TestDocument> testDocuments = new()
        {
            new() { TestProperty = "To Delete 1" },
            new() { TestProperty = "To Delete 2" },
            new() { TestProperty = "To Keep 1" },
            new() { TestProperty = "To Keep 2" },
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
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Delete_WhenNoDocumentWithConditionExists_ReturnFalse()
    {
        // Given
        TestDocument testDocument = new() { TestProperty = "Test 1" };
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var wasDeleted = _mongoCollectionWrapperSUT.Delete(doc => doc.TestProperty == string.Empty);

        // Then
        wasDeleted.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().Contain(doc => doc == testDocument);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Delete_WhenNoDocumentExists_ReturnFalse()
    {
        // Given
        
        // When
        var wasDeleted = _mongoCollectionWrapperSUT.Delete(doc => doc.TestProperty == string.Empty);

        // Then
        wasDeleted.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Find_WhenNoDocumentWithConditionExists_ReturnNull()
    {
        // Given
        TestDocument testDocument = new() { TestProperty = "Test 1" };
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var documentFromDb = _mongoCollectionWrapperSUT.Find(doc => doc.TestProperty == "Nonexistent");

        // Then
        documentFromDb.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Find_WhenNoDocumentExists_ReturnNull()
    {
        // Given
        
        // When
        var documentFromDb = _mongoCollectionWrapperSUT.Find(doc => doc.TestProperty == "Nonexistent");

        // Then
        documentFromDb.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Find_WhenDocumentWithConditionExists_ReturnDocument()
    {
        // Given
        TestDocument testDocument = new() { TestProperty = "Test 1" };
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var documentFromDb = _mongoCollectionWrapperSUT.Find(doc => doc.TestProperty == testDocument.TestProperty);

        // Then
        documentFromDb.Should().Be(testDocument);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void Find_WhenMultipleDocumentsWithConditionExist_ReturnTheFirstOne()
    {
        // Given
        List<TestDocument> testDocuments = new()
        {
            new() { TestProperty = "Example 1" },
            new() { TestProperty = "Example 2" },
            new() { TestProperty = "Test 1" },
            new() { TestProperty = "Test 2" },
        };
        _dbFixture.TestCollection.InsertMany(testDocuments);

        // When
        var documentFromDb = _mongoCollectionWrapperSUT.Find(doc => doc.TestProperty!.Contains("Test"));

        // Then
        documentFromDb.Should().Be(testDocuments.Skip(2).First());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateRecord_WhenRecordWithConditionDoesNotExist_ReturnFalseAndDontUpdate()
    {
        // Given
        TestDocument testDocument = new() { TestProperty = "Test" };
        _dbFixture.TestCollection.InsertOne(testDocument);

        TestDocument updatedDocument = new() { TestProperty = "Updated" };

        // When
        var wasUpdatedSuccessfully = _mongoCollectionWrapperSUT.UpdateRecord(updatedDocument, doc => doc.TestProperty == "Nonexistent");

        // Then
        wasUpdatedSuccessfully.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().OnlyContain(doc => doc == testDocument);
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().HaveCount(1);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateRecord_WhenNoRecordExists_ReturnFalseAndDontUpdate()
    {
        // Given
        TestDocument updatedDocument = new() { TestProperty = "Updated" };

        // When
        var wasUpdatedSuccessfully = _mongoCollectionWrapperSUT.UpdateRecord(updatedDocument, doc => true);

        // Then
        wasUpdatedSuccessfully.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateRecord_WhenRecordWithConditionExists_ReturnTrueAndUpdate()
    {
        // Given
        TestDocument testDocument = new() { TestProperty = "Test" };
        _dbFixture.TestCollection.InsertOne(testDocument);

        TestDocument updatedDocument = new() { Id = testDocument.Id, TestProperty = "Updated" };

        // When
        var wasUpdatedSuccessfully = _mongoCollectionWrapperSUT.UpdateRecord(updatedDocument, doc => doc.Id == testDocument.Id);

        // Then
        wasUpdatedSuccessfully.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().OnlyContain(doc => doc.Id == testDocument.Id && doc.TestProperty == updatedDocument.TestProperty);
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().HaveCount(1);
    }
}
