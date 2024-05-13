using AutoMapper;
using HsnSoft.Base.Application.Services;
using HsnSoft.Base.Domain.Entities.Events;
using HsnSoft.Base.EventBus;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Hhs.IdentityService.Application;

public abstract class ApplicationServiceBase : ApplicationService, IEventApplicationService
{
    [NotNull]
    protected IMapper Mapper { get; }

    [NotNull]
    protected IEventBus EventBus { get; }

    [CanBeNull]
    protected ParentMessageEnvelope ParentIntegrationEvent { get; private set; }

    protected ApplicationServiceBase(IServiceProvider provider) : base(provider)
    {
        var serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider), "ApplicationServiceBase IServiceProvider is null");
        Mapper = serviceProvider.GetRequiredService<IMapper>();
        EventBus = serviceProvider.GetRequiredService<IEventBus>();
    }

    public void SetParentIntegrationEvent<T>(MessageEnvelope<T> @event) where T : IIntegrationEventMessage
    {
        ParentIntegrationEvent = JsonConvert.DeserializeObject<ParentMessageEnvelope>(JsonConvert.SerializeObject(@event));
    }
}