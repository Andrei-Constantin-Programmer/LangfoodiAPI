using FluentValidation;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using System.Text.RegularExpressions;

namespace RecipeSocialMediaAPI.Core.OptionValidators;

public sealed partial class MongoDatabaseOptionValidator : AbstractValidator<MongoDatabaseOptions>
{
    public MongoDatabaseOptionValidator()
    {
        RuleFor(x => x.ConnectionString)
            .NotEmpty();

        RuleFor(x => x.ClusterName)
            .NotEmpty();

        RuleFor(x => x.ConnectionString)
            .Must(connString => ValidMongoDBConnectionString().IsMatch(connString))
            .WithMessage($"{nameof(MongoDatabaseOptions.ConnectionString)} must be a valid connection string, e.g. mongodb+srv://teamname:password@mongolink");
    }

    [GeneratedRegex("^(mongodb\\+srv://|mongodb://)\\S+$")]
    private static partial Regex ValidMongoDBConnectionString();
}