using Hhs.IdentityService.Domain.Enums;
using HsnSoft.Base.Application.Dtos;
using JetBrains.Annotations;

namespace Hhs.IdentityService.Application.Contracts.FakeDomain;

public sealed class FakeDto : EntityDto<Guid>
{
    public DateTime FakeDate { get; set; }

    [NotNull]
    public string FakeCode { get; set; }

    public FakeState FakeState { get; internal set; }
}