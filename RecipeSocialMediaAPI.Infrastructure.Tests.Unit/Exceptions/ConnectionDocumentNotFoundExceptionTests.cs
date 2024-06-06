using FluentAssertions;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Text.Json;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Exceptions;

public class ConnectionDocumentNotFoundExceptionTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void ConnectionDocumentNotFoundException_IsCorrectlySerializedAndDeserialized()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1"
        };

        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2"
        };

        ConnectionDocumentNotFoundException exception = new(user1, user2);

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
