using Hhs.IdentityService.Domain.AppUserDomain.Entities;
using HsnSoft.Base;

namespace Hhs.IdentityService.Domain.AppUserDomain.Exceptions;

[Serializable]
internal sealed class AppUserEmailDuplicateException : BusinessException
{
    public AppUserEmailDuplicateException(string email)
        : base(errorMessage: DomainErrorCodes.AppUserEmailDuplicate)
    {
        ErrorCode = DomainErrorCodes.AppUserEmailDuplicate.Split(':').LastOrDefault();
        WithData($"{nameof(AppUser)}:{nameof(AppUser.Email)}", email);
    }
}