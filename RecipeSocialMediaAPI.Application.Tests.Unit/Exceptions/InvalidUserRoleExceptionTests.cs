using FluentAssertions;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Text.Json;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Exceptions;

public class InvalidUserRoleExceptionTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.INFRASTRUCTURE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void InvalidUserRoleException_IsCorrectlySerializedAndDeserialized()
    {
        // Given
        var role = "User";
        InvalidUserRoleException exception = new(role);

        CustomExceptionSerializationData serializedData = new(exception.Message, exception.HResult, exception.Source, exception.StackTrace, exception.InvalidRole);

        // When
        var json = JsonSerializer.Serialize(serializedData);

        // Then
        var deserializedData = JsonSerializer.Deserialize<CustomExceptionSerializationData>(json);

        deserializedData?.Message.Should().Be(exception.Message);
        deserializedData?.HResult.Should().Be(exception.HResult);
        deserializedData?.Source.Should().Be(exception.Source);
        deserializedData?.StackTrace.Should().Be(exception.StackTrace);

        deserializedData?.InvalidRole.Should().Be(role);
    }

    private record CustomExceptionSerializationData(string Message, int HResult, string? Source, string? StackTrace, string InvalidRole)
        : ExceptionSerializationData(Message, HResult, Source, StackTrace);
}
