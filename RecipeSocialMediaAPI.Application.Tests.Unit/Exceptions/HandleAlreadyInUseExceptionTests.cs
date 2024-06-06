using FluentAssertions;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Text.Json;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Exceptions;

public class HandleAlreadyInUseExceptionTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void HandleAlreadyInUseException_IsCorrectlySerializedAndDeserialized()
    {
        // Given
        var handle = "test_handler";
        HandleAlreadyInUseException exception = new(handle);

        CustomExceptionSerializationData serializedData = new(exception.Message, exception.HResult, exception.Source, exception.StackTrace, handle);

        // When
        var json = JsonSerializer.Serialize(serializedData);

        // Then
        var deserializedData = JsonSerializer.Deserialize<CustomExceptionSerializationData>(json);

        deserializedData?.Message.Should().Be(exception.Message);
        deserializedData?.HResult.Should().Be(exception.HResult);
        deserializedData?.Source.Should().Be(exception.Source);
        deserializedData?.StackTrace.Should().Be(exception.StackTrace);

        deserializedData?.Handle.Should().Be(handle);
    }

    private record CustomExceptionSerializationData(string Message, int HResult, string? Source, string? StackTrace, string Handle)
        : ExceptionSerializationData(Message, HResult, Source, StackTrace);
}
