using Hhs.Shared.Hosting;
using HsnSoft.Base.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Hhs.IdentityService.Controllers.Base;

public sealed class HomeController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly IBaseLogger _logger;

    public HomeController(IWebHostEnvironment environment, IBaseLogger logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (!_environment.IsHhsProduction())
        {
            // only show in development
            return Redirect("/swagger");
        }

        _logger.LogInformation("swagger is disabled in production. Returning 404");
        return NotFound();
    }
}