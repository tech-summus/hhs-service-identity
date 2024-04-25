using HsnSoft.Base.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Hhs.IdentityService.Controllers.Base;

[Produces("application/json")]
[Area("identity-service")]
[ApiController]
public abstract class IdentityServiceController : ApiControllerBase
{
    protected IdentityServiceController(IServiceProvider provider)
    {
        ServiceProvider = provider;
    }
}