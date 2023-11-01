using System.ComponentModel.DataAnnotations;

namespace RecipeSocialMediaAPI.DataAccess.Helpers;

public class MongoDatabaseOptions
{
    public const string CONFIGURATION_SECTION = "MongoDB";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;
    [Required]
    public string ClusterName { get; set; } = string.Empty;
}
