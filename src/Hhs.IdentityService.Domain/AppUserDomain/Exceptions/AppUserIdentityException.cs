using HsnSoft.Base;
using HsnSoft.Base.Validation.Localization;
using Microsoft.AspNetCore.Identity;

namespace Hhs.IdentityService.Domain.AppUserDomain.Exceptions;

[Serializable]
internal sealed class AppUserIdentityException : BusinessException
{
    public AppUserIdentityException(IEnumerable<IdentityError> errors)
        : base(errorMessage: DomainErrorCodes.AppUserIdentityError)
    {
        ErrorCode = DomainErrorCodes.AppUserIdentityError.Split(':').LastOrDefault();
        foreach (var error in errors)
        {
            WithData(ValidationResourceKeys.ErrorReference + (string.IsNullOrWhiteSpace(error.Code)
                    ? ""
                    : " " + error.Code),
                error.Description ?? string.Empty);
        }
    }
}