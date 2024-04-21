using System.ComponentModel.DataAnnotations;
using Hhs.IdentityService.Domain.Enums;
using Hhs.IdentityService.Domain.FakeDomain;
using HsnSoft.Base;
using HsnSoft.Base.Validation.Localization;
using JetBrains.Annotations;

namespace Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Submits;

public sealed class FakeUpdateDto : IValidatableObject
{
    public Guid Id { get; set; }

    public DateTime? FakeDate { get; set; }

    [CanBeNull]
    public string FakeCode { get; set; }

    public FakeState? FakeState { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!FakeDate.HasValue)
        {
            yield return new ValidationResult(ValidationResourceKeys.IsNotEmpty, new[] { "Fake:FakeDate" });
        }

        if (!CheckSafe.NotNullOrEmpty(FakeCode, nameof(FakeCode)))
        {
            yield return new ValidationResult(ValidationResourceKeys.Required, new[] { "Fake:FakeCode" });
        }
        else if (!CheckSafe.Length(FakeCode, nameof(FakeCode), FakeConsts.FakeCodeMaxLength))
        {
            yield return new ValidationResult(ValidationResourceKeys.MaxLength, new[] { "Fake:FakeCode" });
        }

        if (!FakeState.HasValue)
        {
            yield return new ValidationResult(ValidationResourceKeys.IsNotEmpty, new[] { "Fake:FakeState" });
        }
    }
}