using HsnSoft.Base.Application.Dtos;
using JetBrains.Annotations;

namespace Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos;

public sealed class FakeSearchDto : EntityDto<Guid>
{
    [NotNull]
    public string FakeCode { get; set; }
}