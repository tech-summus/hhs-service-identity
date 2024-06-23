using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hhs.IdentityService.Application;
using Hhs.IdentityService.Domain.AppRoleDomain.Entities;
using Hhs.IdentityService.Domain.AppUserDomain.Entities;
using Hhs.IdentityService.EntityFrameworkCore;
using Hhs.IdentityService.EntityFrameworkCore.Context;
using Hhs.Shared.Hosting;
using Hhs.Shared.Hosting.Microservices;
using Hhs.Shared.Hosting.Microservices.Middlewares;
using Microsoft.AspNetCore.Identity;

namespace Hhs.IdentityService;

public sealed class Startup
{
    private IConfiguration Configuration { get; }
    private IWebHostEnvironment WebHostEnvironment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        WebHostEnvironment = environment;
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.ConfigureMicroserviceHost()
            .AddAdvancedController(Configuration, typeof(Startup))
            .AddJwtServerAuthentication(Configuration, WebHostEnvironment, "audience-service-identity")
            .AddAuthorization()
            .AddMicroserviceEventBus(Configuration, typeof(EventHandlersAssemblyMarker).Assembly)
            .AddServiceApplicationConfiguration(Configuration)
            .AddServiceDatabaseConfiguration(Configuration);

        services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.AllowedUserNameCharacters = "abcçdefghiıjklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._@+'#!/^%{}*";
            })
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();

        if (!WebHostEnvironment.IsHhsProduction())
        {
            SwaggerConfigurationHelper.ConfigureWithBearer(services,
                "Please enter a valid token. Token audiences contains audience-service-identity",
                $"{Program.AppName} API");
        }

        var container = new ContainerBuilder();
        container.Populate(services);

        return new AutofacServiceProvider(container.Build());
    }

    public void Configure(IApplicationBuilder app, IHostApplicationLifetime hostApplicationLifetime)
    {
        if (!WebHostEnvironment.IsHhsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Program.AppName} API");
            });
        }

        /*Middleware*/
        app.UseMiddleware<RequestResponseLoggerMiddleware>();
        app.UseLocalizationMiddleware();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            if (!WebHostEnvironment.IsHhsProduction())
            {
                endpoints.MapDefaultControllerRoute();
            }
            else
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", () => $"HHS {Program.AppName} | {WebHostEnvironment.EnvironmentName} | v1.0.0");
            }
        });

        // Subscribe all event handlers
        app.UseEventBus(typeof(EventHandlersAssemblyMarker).Assembly);

        hostApplicationLifetime.ApplicationStopping.Register(OnShutdown);
    }

    private static void OnShutdown() => Console.WriteLine("Stopping web host ({0})...", Program.AppName);
}