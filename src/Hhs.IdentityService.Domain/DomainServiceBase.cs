using HsnSoft.Base.Domain.Services;

namespace Hhs.IdentityService.Domain;

public abstract class DomainServiceBase : DomainService
{
    protected DomainServiceBase(IServiceProvider provider) : base(provider)
    {
    }
}