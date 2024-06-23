using HsnSoft.Base;

namespace Hhs.IdentityService.Domain.AppUserDomain.Exceptions;

[Serializable]
internal sealed class AppUserInvalidTenantDomainException : BusinessException
{
    public AppUserInvalidTenantDomainException(string tenantDomain)
        : base(errorMessage: DomainErrorCodes.InvalidTenantDomainError)
    {
        ErrorCode = DomainErrorCodes.InvalidTenantDomainError.Split(':').LastOrDefault();
        WithData("TenantDomain", tenantDomain ?? string.Empty);
    }
}