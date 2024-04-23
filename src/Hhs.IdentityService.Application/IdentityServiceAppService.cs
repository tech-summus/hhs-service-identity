using AutoMapper;
using HsnSoft.Base.Application.Services;
using HsnSoft.Base.DependencyInjection;
using HsnSoft.Base.Domain.Entities.Events;
using HsnSoft.Base.EventBus;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Hhs.IdentityService.Application;

public abstract class IdentityServiceAppService : BaseApplicationService, IEventApplicationService
{
    [NotNull]
    protected ILogger Logger { get; }

    [NotNull]
    protected IMapper Mapper { get; }

    [NotNull]
    private IEventBus EventBus { get; }

    [CanBeNull]
    private ParentMessageEnvelope ParentIntegrationEvent { get; set; }

    protected IdentityServiceAppService(IBaseLazyServiceProvider provider)
    {
        LazyServiceProvider = provider;

        var loggerFactory = LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger("ContentService");

        Mapper = LazyServiceProvider.LazyGetRequiredService<IMapper>();
        EventBus = LazyServiceProvider.LazyGetRequiredService<IEventBus>();
    }

    public void SetParentIntegrationEvent<T>(MessageEnvelope<T> @event) where T : IIntegrationEventMessage
    {
        ParentIntegrationEvent = new ParentMessageEnvelope
        {
            HopLevel = @event.HopLevel,
            MessageId = @event.MessageId,
            CorrelationId = @event.CorrelationId,
            UserId = @event.UserId,
            UserRoleUniqueName = @event.UserRoleUniqueName,
            Channel = @event.Channel,
            Producer = @event.Producer
        };
    }

    protected async Task PublishEventAsync(IIntegrationEventMessage eventMessage)
        => await EventBus?.PublishAsync(eventMessage, ParentIntegrationEvent)!;
}