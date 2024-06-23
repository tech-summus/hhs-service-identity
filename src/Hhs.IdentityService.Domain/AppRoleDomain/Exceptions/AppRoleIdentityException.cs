using HsnSoft.Base;
using HsnSoft.Base.Validation.Localization;
using Microsoft.AspNetCore.Identity;

namespace Hhs.IdentityService.Domain.AppRoleDomain.Exceptions;

[Serializable]
internal sealed class AppRoleIdentityException : BusinessException
{
    public AppRoleIdentityException(IEnumerable<IdentityError> errors)
        : base(errorMessage: DomainErrorCodes.AppRoleIdentityError)
    {
        ErrorCode = DomainErrorCodes.AppRoleIdentityError.Split(':').LastOrDefault();
        foreach (var error in errors)
        {
            WithData(ValidationResourceKeys.ErrorReference + (string.IsNullOrWhiteSpace(error.Code)
                    ? ""
                    : " " + error.Code),
                error.Description ?? string.Empty);
        }
    }
}