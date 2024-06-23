using System.Globalization;
using System.Text;
using Hhs.IdentityService.Domain.AppRoleDomain.Consts;
using HsnSoft.Base;
using HsnSoft.Base.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace Hhs.IdentityService.Domain.AppRoleDomain.Entities;

public sealed class AppRole : IdentityRole<Guid>, ISoftDelete, IMultiTenant
{
    public bool IsDeleted { get; set; }

    public Guid TenantId { get; private set; }

    public string TenantDomain { get; private set; }

    public bool IsDefault { get; internal set; }
    public bool IsStatic { get; internal set; }
    public bool IsPublic { get; internal set; }

    private AppRole()
    {
        Name = string.Empty;
    }

    internal AppRole(Guid id,
        string name,
        bool isDefault = false,
        bool isStatic = false,
        bool isPublic = false,
        Guid tenantId = default,
        string tenantDomain = null) : this()
    {
        Id = id;
        SetTenant(tenantId, tenantDomain);

        SetName(name);
        IsDefault = isDefault;
        IsStatic = isStatic;
        IsPublic = isPublic;
    }

    internal void SetTenant(Guid tenantId, string tenantDomain)
    {
        TenantId = tenantId;
        TenantDomain = Check.NotNullOrWhiteSpace(tenantDomain, nameof(tenantDomain), AppRoleConsts.TenantDomainMaxLength).ToLower();
    }

    internal void SetName(string name)
    {
        var checkRoleName = Check.NotNullOrWhiteSpace(name, nameof(name), AppRoleConsts.NameMaxLength);

        Name = string.Join("", checkRoleName.ToLower().Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

        NormalizedName = string.Join("", Name.ToUpper().Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
    }
}