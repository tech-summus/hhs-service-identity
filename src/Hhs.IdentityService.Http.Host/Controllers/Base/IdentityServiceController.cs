using HsnSoft.Base.AspNetCore.Mvc;
using HsnSoft.Base.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace Hhs.IdentityService.Controllers.Base;

[Produces("application/json")]
[Area("identity-service")]
[ApiController]
public abstract class IdentityServiceController : ApiControllerBase
{
    protected IdentityServiceController(IBaseLazyServiceProvider provider)
    {
        LazyServiceProvider = provider;
    }
}