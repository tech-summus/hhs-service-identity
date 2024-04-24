using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Hhs.IdentityService.Application;
using Hhs.IdentityService.Application.Contracts.FakeDomain;
using Hhs.IdentityService.Domain;
using Hhs.Shared.Hosting;
using Hhs.Shared.Hosting.Microservices;
using Hhs.Shared.Hosting.Microservices.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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
            .AddMicroserviceMvc(Configuration, typeof(Startup))
            .AddMicroserviceEventBus(Configuration, typeof(EventHandlersAssemblyMarker).Assembly)
            .AddMicroserviceHealthChecks(Configuration, IdentityServiceDbProperties.ConnectionStringName);

        services.Configure<FakeSettings>(Configuration.GetSection("FakeSettings"));

        AddIdentityServiceInfrastructures(services);

        if (!WebHostEnvironment.IsProduction())
        {
            SwaggerConfigurationHelper.Configure(services, "Identity Service API");
        }

        var container = new ContainerBuilder();
        container.Populate(services);

        return new AutofacServiceProvider(container.Build());
    }

    public void Configure(IApplicationBuilder app)
    {
        if (!WebHostEnvironment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API");
            });
        }

        /*Middleware*/
        app.UseMiddleware<RequestResponseLoggerMiddleware>();
        app.UseLocalizationMiddleware();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            if (!WebHostEnvironment.IsProduction())
            {
                endpoints.MapDefaultControllerRoute();
            }
            else
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", () => $"HHS Identity Service | {WebHostEnvironment.EnvironmentName} | v1.0.0");
            }

            endpoints.MapHealthChecks("/hc", new HealthCheckOptions { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });
        });

        // Subscribe all event handlers
        app.UseEventBus(typeof(EventHandlersAssemblyMarker).Assembly);
    }

    private void AddIdentityServiceInfrastructures(IServiceCollection services)
    {
        var assemblyName = typeof(Program).Assembly.GetName().Name;

        services.AddServiceApplicationConfiguration();

        services.AddServiceDatabaseConfiguration(assemblyName, Configuration.GetConnectionString(IdentityServiceDbProperties.ConnectionStringName));
    }
}