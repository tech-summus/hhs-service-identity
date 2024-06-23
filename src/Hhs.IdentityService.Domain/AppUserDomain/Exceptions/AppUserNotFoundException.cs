using HsnSoft.Base;
using HsnSoft.Base.Validation.Localization;

namespace Hhs.IdentityService.Domain.AppUserDomain.Exceptions;

[Serializable]
internal sealed class AppUserNotFoundException : BusinessException
{
    public AppUserNotFoundException(string referenceCode)
        : base(errorMessage: DomainErrorCodes.AppUserNotFound)
    {
        ErrorCode = DomainErrorCodes.AppUserNotFound.Split(':').LastOrDefault();
        WithData(ValidationResourceKeys.ErrorReference, referenceCode);
    }
}