using FluentAssertions;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.Tests.Shared.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Text.Json;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Exceptions;

public class DocumentAlreadyExistsExceptionTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void DocumentAlreadyExistsException_IsCorrectlySerializedAndDeserialized()
    {
        // Given
        TestDocument document = new("Test prop", "1");

        DocumentAlreadyExistsException<TestDocument> exception = new(document);

        ExceptionSerializationData serializedData = new(exception.Message, exception.HResult, exception.Source, exception.StackTrace, exception.Document);

        // When
        var json = JsonSerializer.Serialize(serializedData);

        // Then
        var deserializedData = JsonSerializer.Deserialize<ExceptionSerializationData>(json);

        deserializedData?.Message.Should().Be(exception.Message);
        deserializedData?.HResult.Should().Be(exception.HResult);
        deserializedData?.Source.Should().Be(exception.Source);
        deserializedData?.StackTrace.Should().Be(exception.StackTrace);

        deserializedData?.Document.Should().BeEquivalentTo(document);
    }

    private record ExceptionSerializationData(string Message, int HResult, string? Source, string? StackTrace, TestDocument Document);
}
