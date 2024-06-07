using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Group")]
[ExcludeFromCodeCoverage(Justification = "Unnecessary testing on DTO")]
public record GroupDocument(
    string GroupName,
    string GroupDescription,
    List<string> UserIds,
    string? Id = null
) : MongoDocument(Id);
