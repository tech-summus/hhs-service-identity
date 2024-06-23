using HsnSoft.Base;

namespace Hhs.IdentityService.Domain.AppRoleDomain.Exceptions;

[Serializable]
internal sealed class AppRoleInvalidTenantDomainException : BusinessException
{
    public AppRoleInvalidTenantDomainException(string tenantDomain)
        : base(errorMessage: DomainErrorCodes.InvalidTenantDomainError)
    {
        ErrorCode = DomainErrorCodes.InvalidTenantDomainError.Split(':').LastOrDefault();
        WithData("TenantDomain", tenantDomain ?? string.Empty);
    }
}