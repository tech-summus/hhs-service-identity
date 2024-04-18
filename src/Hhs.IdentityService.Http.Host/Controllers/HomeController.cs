using Microsoft.AspNetCore.Mvc;

namespace Hhs.IdentityService.Controllers;

public sealed class HomeController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger _logger;

    public HomeController(IWebHostEnvironment environment, ILogger<HomeController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (!_environment.IsProduction())
        {
            // only show in development
            return Redirect("/swagger");
        }

        _logger.LogInformation("swagger is disabled in production. Returning 404");
        return NotFound();
    }
}