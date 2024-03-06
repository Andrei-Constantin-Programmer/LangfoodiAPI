﻿using FluentAssertions;
using MongoDB.Driver;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.DataAccess.Tests.Shared.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Integration.MongoConfiguration;

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
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
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
        var result = (await _mongoCollectionWrapperSUT.GetAll(doc => doc.TestProperty!.StartsWith('1'))).ToList();

        // Then
        result.Should().OnlyContain(doc => doc.TestProperty == "1" || doc.TestProperty == "10");
        result.Should().HaveCount(2);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task GetAll_WhenNoDocumentsMatch_ReturnEmptyListAsync()
    {
        // Given
        List<TestDocument> existingDocuments = new();
        for (int i = 0; i <= 10; i++)
        {
            existingDocuments.Add(new(i.ToString()));
        }

        _dbFixture.TestCollection.InsertMany(existingDocuments);

        // When
        var result = await _mongoCollectionWrapperSUT.GetAll(doc => doc.TestProperty!.StartsWith('a'));

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task GetAll_WhenThereAreNoDocuments_ReturnEmptyListAsync()
    {
        // Given

        // When
        var result = await _mongoCollectionWrapperSUT.GetAll(_ => true);

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Insert_AddsDocumentToTheDatabaseAndReturnsDocumentAsync()
    {
        // Given
        TestDocument testDocument = new("Test 1");

        // When
        var document = await _mongoCollectionWrapperSUT.Insert(testDocument);

        // Then
        document.Should().Be(testDocument);
        var documentFromDb = _dbFixture.TestCollection.Find(doc => doc.Id == testDocument.Id).First();
        documentFromDb.Should().Be(testDocument);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Insert_WhenDocumentWithIdAlreadyExists_ThrowDocumentAlreadyExistsExceptionAsync()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var action = async () => await _mongoCollectionWrapperSUT.Insert(testDocument);

        // Then
        await action.Should().ThrowAsync<DocumentAlreadyExistsException<TestDocument>>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Delete_WhenDocumentWithConditionExists_DeleteDocumentAndReturnTrueAsync()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var wasDeleted = await _mongoCollectionWrapperSUT.Delete(doc => doc.Id == testDocument.Id);

        // Then
        wasDeleted.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Delete_WhenMultipleDocumentsWithConditionExist_DeleteFirstFittingDocumentAndReturnTrueAsync()
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
        var wasDeleted = await _mongoCollectionWrapperSUT.Delete(doc => doc.TestProperty!.Contains("To Delete"));

        // Then
        wasDeleted.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList()
            .Should().NotContain(doc => doc.TestProperty == "To Delete 1")
            .And.HaveCount(3);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Delete_WhenNoDocumentWithConditionExists_ReturnFalseAsync()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var wasDeleted = await _mongoCollectionWrapperSUT.Delete(doc => doc.TestProperty == string.Empty);

        // Then
        wasDeleted.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().Contain(doc => doc == testDocument);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Delete_WhenNoDocumentExists_ReturnFalseAsync()
    {
        // Given
        
        // When
        var wasDeleted = await _mongoCollectionWrapperSUT.Delete(doc => doc.TestProperty == string.Empty);

        // Then
        wasDeleted.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Find_WhenNoDocumentWithConditionExists_ReturnNullAsync()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var documentFromDb = await _mongoCollectionWrapperSUT.Find(doc => doc.TestProperty == "Nonexistent");

        // Then
        documentFromDb.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Find_WhenNoDocumentExists_ReturnNullAsync()
    {
        // Given
        
        // When
        var documentFromDb = await _mongoCollectionWrapperSUT.Find(doc => doc.TestProperty == "Nonexistent");

        // Then
        documentFromDb.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Find_WhenDocumentWithConditionExists_ReturnDocumentAsync()
    {
        // Given
        TestDocument testDocument = new("Test 1");
        _dbFixture.TestCollection.InsertOne(testDocument);

        // When
        var documentFromDb = await _mongoCollectionWrapperSUT.Find(doc => doc.TestProperty == testDocument.TestProperty);

        // Then
        documentFromDb.Should().Be(testDocument);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task Find_WhenMultipleDocumentsWithConditionExist_ReturnTheFirstOneAsync()
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
        var documentFromDb = await _mongoCollectionWrapperSUT.Find(doc => doc.TestProperty!.Contains("Test"));

        // Then
        documentFromDb.Should().Be(testDocuments.Skip(2).First());
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateRecord_WhenRecordWithConditionDoesNotExist_ReturnFalseAndDontUpdateAsync()
    {
        // Given
        TestDocument testDocument = new("Test");
        _dbFixture.TestCollection.InsertOne(testDocument);

        TestDocument updatedDocument = new("Updated");

        // When
        var wasUpdatedSuccessfully = await _mongoCollectionWrapperSUT.UpdateRecord(updatedDocument, doc => doc.TestProperty == "Nonexistent");

        // Then
        wasUpdatedSuccessfully.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().OnlyContain(doc => doc == testDocument);
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().HaveCount(1);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateRecord_WhenNoRecordExists_ReturnFalseAndDontUpdateAsync()
    {
        // Given
        TestDocument updatedDocument = new("Updated");

        // When
        var wasUpdatedSuccessfully = await _mongoCollectionWrapperSUT.UpdateRecord(updatedDocument, doc => true);

        // Then
        wasUpdatedSuccessfully.Should().BeFalse();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateRecord_WhenRecordWithConditionExists_ReturnTrueAndUpdateAsync()
    {
        // Given
        TestDocument testDocument = new("Test");
        _dbFixture.TestCollection.InsertOne(testDocument);

        TestDocument updatedDocument = new(Id: testDocument.Id, TestProperty: "Updated");

        // When
        var wasUpdatedSuccessfully = await _mongoCollectionWrapperSUT.UpdateRecord(updatedDocument, doc => doc.Id == testDocument.Id);

        // Then
        wasUpdatedSuccessfully.Should().BeTrue();
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().OnlyContain(doc => doc.Id == testDocument.Id && doc.TestProperty == updatedDocument.TestProperty);
        _dbFixture.TestCollection.Find(doc => true).ToList().Should().HaveCount(1);
    }
}
