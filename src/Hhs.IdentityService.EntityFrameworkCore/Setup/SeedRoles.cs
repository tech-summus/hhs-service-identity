using Hhs.Shared.Helper.Consts;

namespace Hhs.IdentityService.EntityFrameworkCore.Setup;

internal static class SeedRoles
{
    public static List<SeedRole> Roles => new()
    {
        new SeedRole { RoleId = Guid.Parse("E2F3F225-A555-49D9-8E8B-CDE113C18C4C"), Name = DefaultRoleNames.SystemAdmin, IsPublic = false, TenantDomain = DefaultDomainNames.System, TenantId = default },
        new SeedRole { RoleId = Guid.Parse("8C7BBD04-D356-4A64-9B86-8D0D04C3C2EA"), Name = DefaultRoleNames.SystemManager, TenantDomain = DefaultDomainNames.System, TenantId = default },
        new SeedRole { RoleId = Guid.Parse("8E19FFE7-5671-44A6-84B3-BD447A75FAEC"), Name = DefaultRoleNames.SystemUser, TenantDomain = DefaultDomainNames.System, TenantId = default }
    };
}

internal sealed class SeedRole
{
    public Guid RoleId { get; set; }
    public string Name { get; set; }

    public bool IsStatic { get; set; } = true; // can't delete
    public bool IsPublic { get; set; } = true; // view commercial role list
    public bool IsDefault { get; set; } = false; // register screen user

    public Guid TenantId { get; set; } = default;
    public string TenantDomain { get; set; } = null;
}