using HsnSoft.Base;
using HsnSoft.Base.Validation.Localization;

namespace Hhs.IdentityService.Domain.FakeDomain.Exceptions;

[Serializable]
internal sealed class FakeNotFoundException : BusinessException
{
    public FakeNotFoundException(string referenceCode)
        : base(errorMessage: IdentityServiceErrorCodes.FakeNotFound)
    {
        ErrorCode = IdentityServiceErrorCodes.FakeNotFound.Split(':').LastOrDefault();
        WithData(ValidationResourceKeys.ErrorReference, referenceCode);
    }
}