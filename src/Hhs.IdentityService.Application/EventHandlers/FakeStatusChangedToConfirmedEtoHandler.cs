using Hhs.IdentityService.Application.Contracts.Events;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Dtos.Submits;
using Hhs.IdentityService.Application.Contracts.FakeDomain.Interfaces;
using HsnSoft.Base.Domain.Entities.Events;
using HsnSoft.Base.EventBus;
using HsnSoft.Base.EventBus.Logging;

namespace Hhs.IdentityService.Application.EventHandlers;

public class FakeStatusChangedToConfirmedEtoHandler :
    IIntegrationEventHandler<FakeStatusChangedToConfirmedEto>
{
    private readonly IEventBusLogger _logger;
    private readonly IFakeAppService _fakeAppService;

    public FakeStatusChangedToConfirmedEtoHandler(IEventBusLogger logger,
        IFakeAppService fakeAppService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fakeAppService = fakeAppService ?? throw new ArgumentNullException(nameof(fakeAppService));
    }

    public async Task HandleAsync(MessageEnvelope<FakeStatusChangedToConfirmedEto> @event)
    {
        _logger.LogInformation("{Producer} Event[ {EventName} ] => CorrelationId[{CorrelationId}], MessageId[{MessageId}], RelatedMessageId[{RelatedMessageId}]",
            @event.Producer,
            nameof(FakeStatusChangedToConfirmedEto)[..^"Eto".Length],
            @event.CorrelationId ?? string.Empty,
            @event.MessageId.ToString(),
            @event.ParentMessageId != null ? @event.ParentMessageId.Value.ToString() : string.Empty);

        // Simulate a work time for work
        await Task.Delay(5000);

        await _fakeAppService.CreateAsync(new FakeCreateDto { FakeDate = DateTime.UtcNow });

        await Task.CompletedTask;
    }
}