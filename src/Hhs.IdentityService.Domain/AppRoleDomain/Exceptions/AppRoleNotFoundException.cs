using HsnSoft.Base;
using HsnSoft.Base.Validation.Localization;
using Microsoft.Extensions.Localization;

namespace Hhs.IdentityService.Domain.AppRoleDomain.Exceptions;

[Serializable]
internal sealed class AppRoleNotFoundException : BusinessException
{
    public AppRoleNotFoundException(IStringLocalizer localizer, string referenceCode)
        : base(errorMessage: DomainErrorCodes.AppRoleNotFound)
    {
        ErrorCode = DomainErrorCodes.AppRoleNotFound.Split(':').LastOrDefault();
        WithData(ValidationResourceKeys.ErrorReference, referenceCode);
    }
}