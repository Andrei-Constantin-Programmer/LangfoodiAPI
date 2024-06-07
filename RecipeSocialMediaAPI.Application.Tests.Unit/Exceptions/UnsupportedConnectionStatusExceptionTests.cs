using FluentAssertions;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Text.Json;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Exceptions;

public class UnsupportedConnectionStatusExceptionTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void UnsupportedConnectionStatusException_IsCorrectlySerializedAndDeserialized()
    {
        // Given
        string connectionStatus = "Pending";
        UnsupportedConnectionStatusException exception = new(connectionStatus);

        CustomExceptionSerializationData serializedData = new(
            exception.Message,
            exception.HResult,
            exception.Source,
            exception.StackTrace,
            exception.UnsupportedStatus);

        // When
        var json = JsonSerializer.Serialize(serializedData);

        // Then
        var deserializedData = JsonSerializer.Deserialize<CustomExceptionSerializationData>(json);

        deserializedData?.Message.Should().Be(exception.Message);
        deserializedData?.HResult.Should().Be(exception.HResult);
        deserializedData?.Source.Should().Be(exception.Source);
        deserializedData?.StackTrace.Should().Be(exception.StackTrace);

        deserializedData?.UnsupportedStatus.Should().Be(connectionStatus);
    }

    private record CustomExceptionSerializationData(string Message, int HResult, string? Source, string? StackTrace, string UnsupportedStatus)
        : ExceptionSerializationData(Message, HResult, Source, StackTrace);
}
