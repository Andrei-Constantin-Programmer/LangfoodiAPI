using FluentAssertions;
using MongoDB.Driver;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.Helpers;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using RecipeSocialMediaAPI.Infrastructure.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Infrastructure.Tests.Shared.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Integration.MongoConfiguration;

public class MongoCollectionWrapperTests : IClassFixture<MongoDBFixture>
{
    private readonly MongoDBFixture _dbFixture;
    private readonly MongoCollectionWrapper<TestDocument> _mongoCollectionWrapperSUT;

    public MongoCollectionWrapperTests(MongoDBFixture dbFixture)
    {
        _dbFixture = dbFixture;
        _dbFixture.CleanupCollection();

        MongoDatabaseOptions testConfiguration = new()
        {
            ConnectionString = dbFixture.ConnectionString,
            ClusterName = dbFixture.DatabaseName
        };
        
        _mongoCollectionWrapperSUT = new MongoCollectionWrapper<TestDocument>(testConfiguration);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetAll_WhenThereAreDocuments_ReturnAllDocumentsWithProperty()
    {
        // Given
        List<TestDocument> existingDocuments = new();
        for(int i = 0; i <= 10; i++)
        {
            existingDocuments.Add(new(i.ToString()));
        }

        _dbFixture.TestCollection.InsertMany(existingDocuments);

        // When
        var result = (await _mongoCollectionWrapperSUT.GetAllAsync(doc => doc.TestProperty!.StartsWith('1'))).ToList();

        // Then
        result.Should().OnlyContain(doc => doc.TestProperty == "1" || doc.TestProperty == "10");
        result.Should().HaveCount(2);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetAll_WhenNoDocumentsMatch_ReturnEmptyList()
    {
        // Given
        List<TestDocument> existingDocuments = new();
        for (int i = 0; i <= 10; i++)
        {
            existingDocuments.Add(new(i.ToString()));
        }

        _dbFixture.TestCollection.InsertMany(existingDocuments);

        // When
        var result = await _mongoCollectionWrapperSUT.GetAllAsync(doc => doc.TestProperty!.StartsWith('a'));

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetAll_WhenThereAreNoDocuments_ReturnEmptyList()
    {
        // Given

        // When
        var result = await _mongoCollectionWrapperSUT.GetAllAsync(_ => true);

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Insert_AddsDocumentToTheDatabaseAndReturnsDocument()
    {
        // Given
        TestDocument testDocument = new("Test 1");

        // When
        var document = await _mongoCollectionWrapperSUT.InsertAsync(testDocument);

        // Then
        document.Should().Be(testDocument);
        var documentFromDb = _dbFixture.TestCollection.Find(doc => doc.Id == testDocument.Id).First();
        documentFromDb.Should().Be(testDocument);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Insert_WhenDocumentWithIdAlreadyExists_ThrowDocumentAlreadyExistsException()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var action = async () => await _mongoCollectionWrapperSUT.InsertAsync(testDocument);

        // Then
        await action.Should().ThrowAsync<DocumentAlreadyExistsException<TestDocument>>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Delete_WhenDocumentWithConditionExists_DeleteDocumentAndReturnTrue()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var wasDeleted = await _mongoCollectionWrapperSUT.DeleteAsync(doc => doc.Id == testDocument.Id);

        // Then
        wasDeleted.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Delete_WhenMultipleDocumentsWithConditionExist_DeleteFirstFittingDocumentAndReturnTrue()
    {
        // Given
        List<TestDocument> testDocuments = new()
        {
            new("To Delete 1"),
            new("To Delete 2"),
            new("To Keep 1"),
            new("To Keep 2"),
        };
        _dbFixture.TestCollection.InsertMany(testDocuments);

        // When
        var wasDeleted = await _mongoCollectionWrapperSUT.DeleteAsync(doc => doc.TestProperty!.Contains("To Delete"));

        // Then
        wasDeleted.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList()
            .Should().NotContain(doc => doc.TestProperty == "To Delete 1")
            .And.HaveCount(3);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Delete_WhenNoDocumentWithConditionExists_ReturnFalse()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var wasDeleted = await _mongoCollectionWrapperSUT.DeleteAsync(doc => doc.TestProperty == string.Empty);

        // Then
        wasDeleted.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().Contain(doc => doc == testDocument);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Delete_WhenNoDocumentExists_ReturnFalse()
    {
        // Given
        
        // When
        var wasDeleted = await _mongoCollectionWrapperSUT.DeleteAsync(doc => doc.TestProperty == string.Empty);

        // Then
        wasDeleted.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Find_WhenNoDocumentWithConditionExists_ReturnNull()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var documentFromDb = await _mongoCollectionWrapperSUT.GetOneAsync(doc => doc.TestProperty == "Nonexistent");

        // Then
        documentFromDb.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Find_WhenNoDocumentExists_ReturnNull()
    {
        // Given
        
        // When
        var documentFromDb = await _mongoCollectionWrapperSUT.GetOneAsync(doc => doc.TestProperty == "Nonexistent");

        // Then
        documentFromDb.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Find_WhenDocumentWithConditionExists_ReturnDocument()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var documentFromDb = await _mongoCollectionWrapperSUT.GetOneAsync(doc => doc.TestProperty == testDocument.TestProperty);

        // Then
        documentFromDb.Should().Be(testDocument);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task Find_WhenMultipleDocumentsWithConditionExist_ReturnTheFirstOne()
    {
        // Given
        List<TestDocument> testDocuments = new()
        {
            new("Example 1"),
            new("Example 2"),
            new("Test 1"),
            new("Test 2"),
        };
        _dbFixture.TestCollection.InsertMany(testDocuments);

        // When
        var documentFromDb = await _mongoCollectionWrapperSUT.GetOneAsync(doc => doc.TestProperty!.Contains("Test"));

        // Then
        documentFromDb.Should().Be(testDocuments.Skip(2).First());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task UpdateRecord_WhenRecordWithConditionDoesNotExist_ReturnFalseAndDontUpdate()
    {
        // Given
        TestDocument testDocument = new("Test");
        _dbFixture.TestCollection.InsertOne(testDocument);

        TestDocument updatedDocument = new("Updated");

        // When
        var wasUpdatedSuccessfully = await _mongoCollectionWrapperSUT.UpdateAsync(updatedDocument, doc => doc.TestProperty == "Nonexistent");

        // Then
        wasUpdatedSuccessfully.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().OnlyContain(doc => doc == testDocument);
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().HaveCount(1);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task UpdateRecord_WhenNoRecordExists_ReturnFalseAndDontUpdate()
    {
        // Given
        TestDocument updatedDocument = new("Updated");

        // When
        var wasUpdatedSuccessfully = await _mongoCollectionWrapperSUT.UpdateAsync(updatedDocument, doc => true);

        // Then
        wasUpdatedSuccessfully.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task UpdateRecord_WhenRecordWithConditionExists_ReturnTrueAndUpdate()
    {
        // Given
        TestDocument testDocument = new("Test");
        _dbFixture.TestCollection.InsertOne(testDocument);

        TestDocument updatedDocument = new(Id: testDocument.Id, TestProperty: "Updated");

        // When
        var wasUpdatedSuccessfully = await _mongoCollectionWrapperSUT.UpdateAsync(updatedDocument, doc => doc.Id == testDocument.Id);

        // Then
        wasUpdatedSuccessfully.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().OnlyContain(doc => doc.Id == testDocument.Id && doc.TestProperty == updatedDocument.TestProperty);
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().HaveCount(1);
    }
}
