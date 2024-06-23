using System.Globalization;
using System.Text;
using Hhs.IdentityService.Domain.AppUserDomain.Consts;
using Hhs.Shared.Helper.Consts;
using HsnSoft.Base;
using HsnSoft.Base.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace Hhs.IdentityService.Domain.AppUserDomain.Entities;

public sealed class AppUser : IdentityUser<Guid>, ISoftDelete, IMultiTenant
{
    public bool IsDeleted { get; set; }

    public Guid TenantId { get; private set; }

    public string TenantDomain { get; private set; }

    public string Name { get; private set; }

    public string Surname { get; private set; }

    public string DefaultLanguage { get; internal set; }

    public string AvatarSuffixUrl { get; internal set; }

    // public user => IsSystemUser : false, IsTenantUser: false
    public bool IsSystemUser => TenantId == default && (TenantDomain ?? string.Empty).Equals(DefaultDomainNames.System);
    public bool IsTenantUser => TenantId != default && !IsSystemUser;

    private AppUser()
    {
        UserName = string.Empty;
        Email = string.Empty;
    }

    internal AppUser(Guid id, string userName, string email, string phone,
        string name,
        string surname,
        string defaultLanguage,
        string avatarSuffixUrl,
        Guid tenantId = default,
        string tenantDomain = null) : this()
    {
        Id = id;
        SetTenant(tenantId, tenantDomain);
        SetDefaultLanguage(defaultLanguage);

        SetUserName(userName);
        SetEmail(email);
        SetPhone(phone);
        SetName(name);
        SetSurname(surname);
        AvatarSuffixUrl = avatarSuffixUrl;
    }

    internal void SetTenant(Guid tenantId, string tenantDomain)
    {
        TenantId = tenantId;
        TenantDomain = Check.NotNullOrWhiteSpace(tenantDomain, nameof(tenantDomain), AppUserConsts.TenantDomainMaxLength).ToLower();
    }

    internal void SetDefaultLanguage(string defaultLanguage)
    {
        DefaultLanguage = Check.Length(defaultLanguage, nameof(defaultLanguage), AppUserConsts.DefaultLanguageMaxLength);

        var acceptableLanguages = new[] { "en", "ru", "tr" };
        if (!acceptableLanguages.Contains((DefaultLanguage ?? "").ToLower()))
        {
            DefaultLanguage = "en";
        }
    }

    internal void SetUserName(string username)
    {
        var checkUserName = Check.NotNullOrWhiteSpace(username, nameof(username), AppUserConsts.UserNameMaxLength);

        UserName = string.Join("", checkUserName.ToLower().Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

        NormalizedUserName = string.Join("", UserName.ToUpper().Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
    }

    internal void SetEmail(string email)
    {
        var checkEmail = Check.NotNullOrWhiteSpace(email, nameof(email), AppUserConsts.EmailMaxLength);

        Email = string.Join("", checkEmail.ToLower().Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

        NormalizedEmail = string.Join("", Email.ToUpper().Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
    }

    internal void SetPhone(string phone)
    {
        PhoneNumber = Check.Length(phone, nameof(phone), AppUserConsts.PhoneNumberMaxLength);
    }

    internal void SetName(string name)
    {
        Name = Check.Length(name, nameof(name), AppUserConsts.NameMaxLength);
    }

    internal void SetSurname(string surname)
    {
        Surname = Check.Length(surname, nameof(surname), AppUserConsts.SurnameMaxLength);
    }
}