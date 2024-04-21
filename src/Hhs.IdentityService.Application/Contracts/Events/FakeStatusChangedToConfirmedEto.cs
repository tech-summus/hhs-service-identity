using HsnSoft.Base.Domain.Entities.Events;

namespace Hhs.IdentityService.Application.Contracts.Events;

public sealed record FakeStatusChangedToConfirmedEto(Guid OrderId, string OrderStatus) : IIntegrationEventMessage
{
    public Guid OrderId { get; } = OrderId;
    public string OrderStatus { get; } = OrderStatus;
}