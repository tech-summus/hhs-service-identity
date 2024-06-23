using Hhs.IdentityService.Domain.AppRoleDomain.Entities;
using HsnSoft.Base;

namespace Hhs.IdentityService.Domain.AppRoleDomain.Exceptions;

[Serializable]
internal sealed class AppRoleNameDuplicateException : BusinessException
{
    public AppRoleNameDuplicateException(string name)
        : base(errorMessage: DomainErrorCodes.AppRoleNameDuplicate)
    {
        ErrorCode = DomainErrorCodes.AppRoleNameDuplicate.Split(':').LastOrDefault();
        WithData($"{nameof(AppRole)}:{nameof(AppRole.Name)}", name);
    }
}