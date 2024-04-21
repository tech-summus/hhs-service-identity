using HsnSoft.Base.DependencyInjection;
using HsnSoft.Base.Domain.Services;

namespace Hhs.IdentityService.Domain;

public abstract class IdentityServiceManager : DomainService
{
    protected IdentityServiceManager(IBaseLazyServiceProvider provider)
    {
        LazyServiceProvider = provider;
    }
}