using HsnSoft.Base.Domain.Services;

namespace Hhs.IdentityService.Domain;

public abstract class IdentityServiceManager : DomainService
{
    protected IdentityServiceManager(IServiceProvider provider)
    {
        ServiceProvider = provider;
    }
}