using JetBrains.Annotations;

namespace Hhs.IdentityService.Domain;

public static class IdentityServiceDbProperties
{
    public static string DbTablePrefix { get; set; } = "";

    [CanBeNull]
    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "IdentityService";
}