using Mono.Cecil;
using NetArchTest.Rules;

namespace RecipeSocialMediaAPI.Core.Tests.Architecture.TestHelpers;

internal class DoesNotHaveNestedInheritanceRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition typeDefinition)
    {
        var baseType = typeDefinition.BaseType.Resolve();

        return TypeIsBaseClass(baseType)
            || baseType is null
            || TypeIsBaseClass(baseType.BaseType?.Resolve());

        static bool TypeIsBaseClass(TypeDefinition? typeDefinition) =>
            typeDefinition is null ||
            typeDefinition.Name is "Enum" or "Object";
    }
}
