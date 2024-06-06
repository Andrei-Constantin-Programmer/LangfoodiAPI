using FluentAssertions;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Text.Json;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Exceptions;

public class InvalidConnectionStatusExceptionTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void InvalidConnectionStatusException_IsCorrectlySerializedAndDeserialized()
    {
        // Given
        ConnectionDocument document = new("u1", "u2", "Pending", "conn1");

        InvalidConnectionStatusException exception = new(document);

        ExceptionSerializationData serializedData = new(exception.Message, exception.HResult, exception.Source, exception.StackTrace);

        // When
        var json = JsonSerializer.Serialize(serializedData);

        // Then
        var deserializedData = JsonSerializer.Deserialize<ExceptionSerializationData>(json);

        deserializedData?.Message.Should().Be(exception.Message);
        deserializedData?.HResult.Should().Be(exception.HResult);
        deserializedData?.Source.Should().Be(exception.Source);
        deserializedData?.StackTrace.Should().Be(exception.StackTrace);
    }
}
