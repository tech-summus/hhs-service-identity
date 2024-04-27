using AutoMapper;
using HsnSoft.Base.Application.Services;
using HsnSoft.Base.Domain.Entities.Events;
using HsnSoft.Base.EventBus;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Hhs.IdentityService.Application;

public abstract class IdentityServiceAppService : BaseApplicationService, IEventApplicationService
{
    [NotNull]
    protected ILogger Logger { get; }

    [NotNull]
    protected IMapper Mapper { get; }

    [NotNull]
    protected IEventBus EventBus { get; }

    [CanBeNull]
    protected ParentMessageEnvelope ParentIntegrationEvent { get; private set; }

    protected IdentityServiceAppService(IServiceProvider provider)
    {
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger("IdentityService");

        Mapper = provider.GetRequiredService<IMapper>();
        EventBus = provider.GetRequiredService<IEventBus>();
    }

    public void SetParentIntegrationEvent<T>(MessageEnvelope<T> @event) where T : IIntegrationEventMessage
    {
        ParentIntegrationEvent = JsonConvert.DeserializeObject<ParentMessageEnvelope>(JsonConvert.SerializeObject(@event));
    }
}