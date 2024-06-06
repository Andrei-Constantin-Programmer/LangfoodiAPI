namespace RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;

public record ExceptionSerializationData(string Message, int HResult, string? Source, string? StackTrace);