using HsnSoft.Base.Domain.Entities.Events;

namespace Hhs.IdentityService.Application.Contracts.Events;

public sealed record OperationFailedEto(Guid OrderId) : IIntegrationEventMessage
{
    public Guid OrderId { get; } = OrderId;
}