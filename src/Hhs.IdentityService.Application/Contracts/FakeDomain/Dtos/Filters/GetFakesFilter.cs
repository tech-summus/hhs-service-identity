using Hhs.IdentityService.Domain.Enums;
using HsnSoft.Base.Application.Dtos;
using JetBrains.Annotations;

namespace Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Filters;

public sealed class GetFakesFilter : SortedResultRequestDto
{
    [CanBeNull]
    public string FakeCode { get; set; } = null;

    public FakeState? FakeState { get; set; } = null;
}