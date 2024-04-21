using AutoMapper;
using HsnSoft.Base.Application.Services;
using HsnSoft.Base.DependencyInjection;
using HsnSoft.Base.Domain.Entities.Events;
using HsnSoft.Base.EventBus;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Hhs.IdentityService.Application;

public abstract class IdentityServiceAppService : BaseApplicationService
{
    [NotNull]
    protected IMapper Mapper { get; }

    [NotNull]
    private IEventBus EventBus { get; }

    protected IdentityServiceAppService(IBaseLazyServiceProvider provider)
    {
        LazyServiceProvider = provider;
        Mapper = LazyServiceProvider.LazyGetRequiredService<IMapper>();
        EventBus = LazyServiceProvider.LazyGetRequiredService<IEventBus>();
    }

    protected async Task PublishEventAsync(IIntegrationEventMessage eventMessage)
    {
        var loggerFactory = LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("IdentityService");

        logger.LogInformation("----- Publishing integration event: {@IntegrationEvent}", eventMessage);
        try
        {
            await EventBus.PublishAsync(eventMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR Publishing integration event: {PublishError}", ex.Message);
            throw;
        }
    }
}