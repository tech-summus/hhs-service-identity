using Hhs.IdentityService.Domain.AppUserDomain.Entities;
using HsnSoft.Base;

namespace Hhs.IdentityService.Domain.AppUserDomain.Exceptions;

[Serializable]
internal sealed class AppUserUsernameDuplicateException : BusinessException
{
    public AppUserUsernameDuplicateException(string username)
        : base(errorMessage: DomainErrorCodes.AppUserUsernameDuplicate)
    {
        ErrorCode = DomainErrorCodes.AppUserUsernameDuplicate.Split(':').LastOrDefault();
        WithData($"{nameof(AppUser)}:{nameof(AppUser.UserName)}", username);
    }
}