using FluentAssertions;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Text.Json;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Exceptions;

public class UsernameAlreadyInUseExceptionTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void UsernameAlreadyInUseException_IsCorrectlySerializedAndDeserialized()
    {
        // Given
        string username = "Test username";
        UsernameAlreadyInUseException exception = new(username);

        ExceptionSerializationData serializedData = new(
            exception.Message,
            exception.HResult,
            exception.Source,
            exception.StackTrace,
            exception.Username);

        // When
        var json = JsonSerializer.Serialize(serializedData);

        // Then
        var deserializedData = JsonSerializer.Deserialize<ExceptionSerializationData>(json);

        deserializedData?.Message.Should().Be(exception.Message);
        deserializedData?.HResult.Should().Be(exception.HResult);
        deserializedData?.Source.Should().Be(exception.Source);
        deserializedData?.StackTrace.Should().Be(exception.StackTrace);

        deserializedData?.Username.Should().Be(username);
    }

    private record ExceptionSerializationData(string Message, int HResult, string? Source, string? StackTrace, string Username);
}
