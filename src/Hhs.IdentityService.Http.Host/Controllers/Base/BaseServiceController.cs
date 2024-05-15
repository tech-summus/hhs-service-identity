using HsnSoft.Base.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Hhs.IdentityService.Controllers.Base;

[Produces("application/json")]
[Area("identity-service")]
[ApiController]
public abstract class BaseServiceController : ApiControllerBase
{
    protected BaseServiceController(IServiceProvider provider)
    {
        ServiceProvider = provider;
    }
}