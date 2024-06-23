using Hhs.Shared.Helper.Consts;

namespace Hhs.IdentityService.EntityFrameworkCore.Setup;

internal static class SeedUsers
{
    private const string DefaultPlainPassword = "Passw0rd!";

    public static List<SeedUser> Users => new()
    {
        new SeedUser
        {
            UserId = Guid.Parse("4A670F80-5592-44D3-AFD5-C8DFCA3679E4"),
            TenantId = default,
            TenantDomain = DefaultDomainNames.System,
            Username = DefaultRoleNames.SystemAdmin,
            PlainPassword = DefaultPlainPassword,
            GivenName = DefaultDomainNames.System,
            FamilyName = IdentityConsts.Admin,
            Email = $"{DefaultRoleNames.SystemAdmin}@{IdentityConsts.SolutionName}.com",
            Roles = { DefaultRoleNames.SystemAdmin }
        },
        new SeedUser
        {
            UserId = Guid.Parse("57865461-E1FD-4009-9256-A6D2B04367EB"),
            TenantId = default,
            TenantDomain = DefaultDomainNames.System,
            Username = DefaultRoleNames.SystemManager,
            PlainPassword = DefaultPlainPassword,
            GivenName = DefaultDomainNames.System,
            FamilyName = IdentityConsts.Manager,
            Email = $"{DefaultRoleNames.SystemManager}@{IdentityConsts.SolutionName}.com",
            Roles = { DefaultRoleNames.SystemManager }
        },
        new SeedUser
        {
            UserId = Guid.Parse("BA271C49-4FCD-4143-8A17-5B6B7452AF66"),
            TenantId = default,
            TenantDomain = DefaultDomainNames.System,
            Username = DefaultRoleNames.SystemUser,
            PlainPassword = DefaultPlainPassword,
            GivenName = DefaultDomainNames.System,
            FamilyName = IdentityConsts.User,
            Email = $"{DefaultRoleNames.SystemUser}@{IdentityConsts.SolutionName}.com",
            Roles = { DefaultRoleNames.SystemUser }
        }
    };
}

internal sealed class SeedUser
{
    public Guid UserId { get; set; }

    public string Username { get; set; }
    public string PlainPassword { get; set; }

    public Guid TenantId { get; set; } = default;
    public string TenantDomain { get; set; } = null;

    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public List<string> Roles { get; set; } = new();
}